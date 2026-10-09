-- =====================================================================
-- KỊCH BẢN CSDL CHO KIẾN TRÚC HƯỚNG DỊCH VỤ (SOA) QUẢN LÝ ĐỒ ÁN
-- Theo nguyên tắc Database-per-service: Mỗi dịch vụ sở hữu CSDL riêng biệt,
-- KHÔNG có khóa ngoại vật lý (Foreign Key) xuyên cơ sở dữ liệu.
-- =====================================================================

-- =====================================================================
-- 1. CSDL DÀNH CHO SinhVienService: SinhVienDb
-- =====================================================================
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SinhVienDb')
BEGIN
    CREATE DATABASE SinhVienDb;
END
GO

USE SinhVienDb;
GO

IF OBJECT_ID('SINHVIEN', 'U') IS NOT NULL
    DROP TABLE SINHVIEN;
GO

CREATE TABLE SINHVIEN (
    MaSV VARCHAR(20) NOT NULL PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    Lop NVARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15) NOT NULL
);
GO

-- Nạp dữ liệu mẫu ban đầu
INSERT INTO SINHVIEN (MaSV, HoTen, Lop, Email, SoDienThoai) VALUES
('SV001', N'Nguyễn Văn An', N'CNTT1', 'an.nv@university.edu.vn', '0901234567'),
('SV002', N'Trần Thị Bình', N'CNTT1', 'binh.tt@university.edu.vn', '0902345678'),
('SV003', N'Lê Hoàng Cường', N'CNTT2', 'cuong.lh@university.edu.vn', '0903456789'),
('SV004', N'Phạm Minh Đức', N'CNTT2', 'duc.pm@university.edu.vn', '0904567890');
GO

-- =====================================================================
-- 2. CSDL DÀNH CHO DeTaiService: DeTaiDb
-- =====================================================================
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DeTaiDb')
BEGIN
    CREATE DATABASE DeTaiDb;
END
GO

USE DeTaiDb;
GO

IF OBJECT_ID('DETAI', 'U') IS NOT NULL
    DROP TABLE DETAI;
GO

CREATE TABLE DETAI (
    MaDT VARCHAR(20) NOT NULL PRIMARY KEY,
    TenDT NVARCHAR(200) NOT NULL,
    MoTa NVARCHAR(1000) NULL,
    GiangVienHD NVARCHAR(100) NOT NULL,
    SoLuongToiDa INT NOT NULL DEFAULT 2
);
GO

-- Nạp dữ liệu mẫu ban đầu
INSERT INTO DETAI (MaDT, TenDT, MoTa, GiangVienHD, SoLuongToiDa) VALUES
('DT01', N'Hệ thống Quản lý Đồ án theo Kiến trúc SOA', N'Xây dựng hệ thống phân tán chia theo các service độc lập sử dụng .NET 8 Web API và Microservices pattern.', N'TS. Trần Văn Hùng', 2),
('DT02', N'Ứng dụng AI Nhận diện Khuôn mặt trong Điểm danh', N'Tích hợp mô hình Deep Learning xử lý ảnh điểm danh sinh viên tự động theo thời gian thực.', N'ThS. Lê Thị Mai', 2),
('DT03', N'Nền tảng Thương mại Điện tử hỗ trợ Thanh toán Trực tuyến', N'Phát triển sàn giao dịch tích hợp cổng thanh toán VNPay, ZaloPay và quản lý kho hàng.', N'TS. Nguyễn Quốc Bảo', 3);
GO

-- =====================================================================
-- 3. CSDL DÀNH CHO DangKyService: DangKyDb
-- =====================================================================
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DangKyDb')
BEGIN
    CREATE DATABASE DangKyDb;
END
GO

USE DangKyDb;
GO

IF OBJECT_ID('DANGKY', 'U') IS NOT NULL
    DROP TABLE DANGKY;
GO

CREATE TABLE DANGKY (
    MaDK INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    MaSV VARCHAR(20) NOT NULL, -- Logical reference to SinhVienService
    MaDT VARCHAR(20) NOT NULL, -- Logical reference to DeTaiService
    NgayDangKy DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    TrangThai NVARCHAR(50) NOT NULL DEFAULT 'DaDangKy'
);
GO

CREATE INDEX IX_DANGKY_MaSV ON DANGKY(MaSV);
CREATE INDEX IX_DANGKY_MaDT ON DANGKY(MaDT);
GO

-- Nạp dữ liệu mẫu ban đầu (SV001 đã đăng ký DT01)
INSERT INTO DANGKY (MaSV, MaDT, NgayDangKy, TrangThai) VALUES
('SV001', 'DT01', '2026-10-01 08:30:00', 'DaDangKy');
GO
