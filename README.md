# Hệ Thống Quản Lý Đồ Án Tốt Nghiệp (Kiến Trúc Hướng Dịch Vụ - SOA)

Hệ thống được thiết kế theo đúng nguyên tắc **Kiến trúc hướng dịch vụ (Service-Oriented Architecture - SOA)** trên nền tảng **ASP.NET Core Web API (.NET 8)**.

---

## 1. Bối cảnh & Mục tiêu Kiến trúc
- **Dịch vụ độc lập (Autonomous Services)**: Mỗi dịch vụ là một Web API project riêng, sở hữu cơ sở dữ liệu độc lập (Database-per-service), không phụ thuộc mã nguồn hay tham chiếu project (No ProjectReference).
- **Giao tiếp chuẩn REST/JSON qua HTTP**: Các service trao đổi thông tin bằng HTTP REST API thay vì chia sẻ chung database.
- **Không có Foreign Key vật lý xuyên service**: Ràng buộc toàn vẹn được kiểm tra thông qua lời gọi API giữa các service.
- **Khả năng kiên cường (Resilience)**: Tích hợp `IHttpClientFactory` với **Polly** (Retry 3 lần với Exponential Backoff và Timeout 5s).
- **Global Exception Handling**: Bắt lỗi tập trung qua Custom Middleware, trả về lỗi HTTP 500 chuẩn hóa.
- **Structured Logging & Health Check**: Tích hợp Serilog và `/health` endpoint trên từng service.

---

## 2. Sơ đồ Kiến trúc & Phân bố Port

```text
                                  +------------------------------------+
                                  |    WebClient (ASP.NET Core MVC)    |
                                  |         http://localhost:5000      |
                                  +-----------------+------------------+
                                                    |
                                    REST API (HTTP) |
               +------------------------------------+------------------------------------+
               |                                    |                                    |
               v                                    v                                    v
+-------------------------------+   +-------------------------------+   +-------------------------------+
|       SinhVienService         |   |         DeTaiService          |   |        DangKyService          |
|    http://localhost:5001      |   |    http://localhost:5002      |   |    http://localhost:5003      |
|    CSDL: SinhVienDb           |   |    CSDL: DeTaiDb              |   |    CSDL: DangKyDb             |
|    Swagger: /swagger          |   |    Swagger: /swagger          |   |    Swagger: /swagger          |
+---------------+---------------+   +---------------+---------------+   +---------------+---------------+
                ^                                   ^                                   |
                |                                   |          Gọi HTTP xác thực        |
                +-----------------------------------+-----------------------------------+
```

| Tên Service | Port | Loại Project | Vai trò |
|---|---|---|---|
| **WebClient** | `5000` | ASP.NET Core MVC | Giao diện người dùng cho Sinh viên, Đề tài và Đăng ký |
| **SinhVienService**| `5001` | ASP.NET Core Web API | CRUD Sinh viên, kiểm tra ràng buộc trước khi xóa |
| **DeTaiService** | `5002` | ASP.NET Core Web API | CRUD Đề tài, kiểm tra ràng buộc trước khi xóa |
| **DangKyService** | `5003` | ASP.NET Core Web API | Điều phối nghiệp vụ đăng ký đồ án, Typed HttpClient + Polly |

---

## 3. Danh sách Endpoints RESTful

### SinhVienService (Port 5001)
- `GET /health`: Kiểm tra trạng thái hoạt động
- `GET /api/sinhvien`: Lấy danh sách toàn bộ sinh viên (HTTP 200)
- `GET /api/sinhvien/{maSV}`: Lấy chi tiết sinh viên (HTTP 200, 404)
- `POST /api/sinhvien`: Thêm mới sinh viên (HTTP 201, 400, 409)
- `PUT /api/sinhvien/{maSV}`: Cập nhật thông tin sinh viên (HTTP 204, 400, 404)
- `DELETE /api/sinhvien/{maSV}`: Xóa sinh viên (HTTP 204, 404, 409 nếu đang có đồ án)

### DeTaiService (Port 5002)
- `GET /health`: Kiểm tra trạng thái hoạt động
- `GET /api/detai`: Lấy danh sách toàn bộ đề tài (HTTP 200)
- `GET /api/detai/{maDT}`: Lấy chi tiết đề tài (HTTP 200, 404)
- `POST /api/detai`: Thêm mới đề tài (HTTP 201, 400, 409)
- `PUT /api/detai/{maDT}`: Cập nhật thông tin đề tài (HTTP 204, 400, 404)
- `DELETE /api/detai/{maDT}`: Xóa đề tài (HTTP 204, 404, 409 nếu có SV đăng ký)

### DangKyService (Port 5003)
- `GET /health`: Kiểm tra trạng thái hoạt động
- `GET /api/dangky`: Lấy danh sách đăng ký đã được làm giàu dữ liệu (HTTP 200)
- `GET /api/dangky/{id}`: Lấy chi tiết đăng ký (HTTP 200, 404)
- `GET /api/dangky/sinhvien/{maSV}`: Lấy đăng ký theo mã sinh viên
- `GET /api/dangky/detai/{maDT}`: Lấy đăng ký theo mã đề tài
- `POST /api/dangky`: Đăng ký đề tài cho sinh viên (HTTP 201, 400, 404, 409, 503)
- `PUT /api/dangky/{id}`: Cập nhật trạng thái đăng ký (HTTP 204, 400, 404)
- `DELETE /api/dangky/{id}`: Hủy đăng ký đề tài (HTTP 204, 404)

---

## 4. Các Quy tắc Nghiệp vụ đã Thực thi
1. **Mỗi sinh viên chỉ được đăng ký 01 đề tài**: Nếu sinh viên đã có bản ghi đăng ký hợp lệ, DangKyService lập tức từ chối và trả về mã HTTP `409 Conflict`.
2. **Không vượt quá số lượng tối đa của đề tài**: Khi đăng ký, DangKyService gọi sang DeTaiService lấy `SoLuongToiDa`, nếu số lượng đăng ký hiện tại bằng hoặc vượt chỉ tiêu -> Trả về `409 Conflict`.
3. **Kiểm tra tồn tại xuyên service**: Nếu `MaSV` không tồn tại ở SinhVienService hoặc `MaDT` không tồn tại ở DeTaiService -> Trả về `404 Not Found`.
4. **Không cho phép xóa đối tượng đang có ràng buộc**:
   - `SinhVienService` gọi `DangKyService/api/dangky/sinhvien/{maSV}` trước khi xóa. Nếu có -> Trả về `409 Conflict`.
   - `DeTaiService` gọi `DangKyService/api/dangky/detai/{maDT}` trước khi xóa. Nếu có -> Trả về `409 Conflict`.

---

## 5. Hướng dẫn Chạy ứng dụng

### Cách 1: Chạy trực tiếp bằng .NET CLI (Đơn giản nhất để test)
Mở 4 tab terminal riêng biệt và chạy lần lượt các lệnh:

```bash
# Tab 1: Khởi động SinhVienService (cổng 5001)
cd services/SinhVienService
dotnet run --launch-profile http

# Tab 2: Khởi động DeTaiService (cổng 5002)
cd services/DeTaiService
dotnet run --launch-profile http

# Tab 3: Khởi động DangKyService (cổng 5003)
cd services/DangKyService
dotnet run --launch-profile http

# Tab 4: Khởi động WebClient (cổng 5000)
cd clients/WebClient
dotnet run --launch-profile http
```

Mở trình duyệt truy cập:
- Giao diện người dùng: [http://localhost:5000](http://localhost:5000)
- Swagger SinhVien: [http://localhost:5001/swagger](http://localhost:5001/swagger)
- Swagger DeTai: [http://localhost:5002/swagger](http://localhost:5002/swagger)
- Swagger DangKy: [http://localhost:5003/swagger](http://localhost:5003/swagger)

### Cách 2: Chạy toàn bộ bằng Docker Compose
Đảm bảo đã cài Docker Desktop, đứng tại thư mục gốc chạy:
```bash
docker-compose up --build
```

---

## 6. Hướng dẫn Kiểm thử tự động qua file `requests.http`
Dự án đã chuẩn bị sẵn file `requests.http` ở thư mục gốc. 
Bạn có thể mở trong VS Code (với extension REST Client) hoặc Visual Studio để bấm **Send Request** trực tiếp kiểm tra từng kịch bản nghiệp vụ:
- Kiểm tra thêm/sửa/xóa sinh viên, đề tài.
- Kiểm tra trùng lặp khóa chính.
- Kiểm tra đăng ký thành công và đăng ký thất bại do đầy chỗ hoặc trùng sinh viên.
- Kiểm tra tính năng chặn xóa khi đang có ràng buộc liên dịch vụ.
