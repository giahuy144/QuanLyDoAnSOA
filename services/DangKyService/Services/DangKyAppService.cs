using DangKyService.Clients;
using DangKyService.DTOs;
using DangKyService.Models;
using DangKyService.Repositories;

namespace DangKyService.Services;

/// <summary>
/// Service điều phối nghiệp vụ Đăng ký đồ án
/// Thể hiện kiến trúc SOA: Không có Foreign Key vật lý, 
/// Gọi HTTP API sang SinhVienService và DeTaiService để kiểm tra và lấy thông tin.
/// </summary>
public class DangKyAppService : IDangKyService
{
    private readonly IDangKyRepository _repository;
    private readonly ISinhVienClient _sinhVienClient;
    private readonly IDeTaiClient _deTaiClient;
    private readonly ILogger<DangKyAppService> _logger;

    public DangKyAppService(
        IDangKyRepository repository,
        ISinhVienClient sinhVienClient,
        IDeTaiClient deTaiClient,
        ILogger<DangKyAppService> logger)
    {
        _repository = repository;
        _sinhVienClient = sinhVienClient;
        _deTaiClient = deTaiClient;
        _logger = logger;
    }

    public async Task<IEnumerable<DangKyResponseDto>> GetAllDangKysAsync()
    {
        var list = await _repository.GetAllAsync();
        var resultList = new List<DangKyResponseDto>();

        foreach (var item in list)
        {
            var dto = await EnrichDangKyDataAsync(item);
            resultList.Add(dto);
        }

        return resultList;
    }

    public async Task<DangKyResponseDto?> GetDangKyByIdAsync(int maDK)
    {
        var item = await _repository.GetByIdAsync(maDK);
        if (item == null) return null;

        return await EnrichDangKyDataAsync(item);
    }

    public async Task<IEnumerable<DangKyResponseDto>> GetDangKysByMaSVAsync(string maSV)
    {
        var list = await _repository.GetByMaSVAsync(maSV);
        var resultList = new List<DangKyResponseDto>();
        foreach (var item in list)
        {
            resultList.Add(await EnrichDangKyDataAsync(item));
        }
        return resultList;
    }

    public async Task<IEnumerable<DangKyResponseDto>> GetDangKysByMaDTAsync(string maDT)
    {
        var list = await _repository.GetByMaDTAsync(maDT);
        var resultList = new List<DangKyResponseDto>();
        foreach (var item in list)
        {
            resultList.Add(await EnrichDangKyDataAsync(item));
        }
        return resultList;
    }

    public async Task<(bool Success, int StatusCode, string? ErrorMessage, DangKyResponseDto? Result)> CreateDangKyAsync(CreateDangKyDto dto)
    {
        var maSV = dto.MaSV.Trim().ToUpper();
        var maDT = dto.MaDT.Trim().ToUpper();

        // 1. Kiểm tra quy tắc nghiệp vụ: Mỗi sinh viên chỉ đăng ký 1 đề tài (chưa bị hủy)
        if (await _repository.HasStudentRegisteredAsync(maSV))
        {
            return (false, StatusCodes.Status409Conflict, $"Sinh viên '{maSV}' đã đăng ký đề tài rồi. Mỗi sinh viên chỉ được đăng ký 1 đề tài.", null);
        }

        // 2. Giao tiếp liên dịch vụ qua HTTP: Kiểm tra sự tồn tại của Sinh viên ở SinhVienService
        SinhVienDto? sinhVien;
        try
        {
            sinhVien = await _sinhVienClient.GetSinhVienByIdAsync(maSV);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Lỗi kết nối tới SinhVienService.");
            return (false, StatusCodes.Status503ServiceUnavailable, "Không thể kết nối đến SinhVienService để xác thực sinh viên.", null);
        }

        if (sinhVien == null)
        {
            return (false, StatusCodes.Status404NotFound, $"Không tìm thấy sinh viên có mã '{maSV}' trong hệ thống.", null);
        }

        // 3. Giao tiếp liên dịch vụ qua HTTP: Kiểm tra sự tồn tại của Đề tài ở DeTaiService
        DeTaiDto? deTai;
        try
        {
            deTai = await _deTaiClient.GetDeTaiByIdAsync(maDT);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Lỗi kết nối tới DeTaiService.");
            return (false, StatusCodes.Status503ServiceUnavailable, "Không thể kết nối đến DeTaiService để xác thực đề tài.", null);
        }

        if (deTai == null)
        {
            return (false, StatusCodes.Status404NotFound, $"Không tìm thấy đề tài có mã '{maDT}' trong hệ thống.", null);
        }

        // 4. Kiểm tra quy tắc nghiệp vụ: Đề tài không vượt quá SoLuongToiDa
        var currentRegisteredCount = await _repository.CountByMaDTAsync(maDT);
        if (currentRegisteredCount >= deTai.SoLuongToiDa)
        {
            return (false, StatusCodes.Status409Conflict, $"Đề tài '{deTai.TenDT}' đã đủ số lượng tối đa ({deTai.SoLuongToiDa} sinh viên).", null);
        }

        // 5. Tiến hành lưu bản ghi Đăng ký
        var newDangKy = new DangKy
        {
            MaSV = maSV,
            MaDT = maDT,
            NgayDangKy = DateTime.UtcNow,
            TrangThai = "DaDangKy"
        };

        var created = await _repository.AddAsync(newDangKy);
        _logger.LogInformation("Sinh viên {MaSV} đăng ký thành công đề tài {MaDT}", maSV, maDT);

        var result = new DangKyResponseDto
        {
            MaDK = created.MaDK,
            MaSV = created.MaSV,
            MaDT = created.MaDT,
            NgayDangKy = created.NgayDangKy,
            TrangThai = created.TrangThai,
            HoTenSinhVien = sinhVien.HoTen,
            LopSinhVien = sinhVien.Lop,
            TenDeTai = deTai.TenDT,
            GiangVienHD = deTai.GiangVienHD
        };

        return (true, StatusCodes.Status201Created, null, result);
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateTrangThaiAsync(int maDK, UpdateDangKyDto dto)
    {
        var item = await _repository.GetByIdAsync(maDK);
        if (item == null)
        {
            return (false, $"Không tìm thấy đăng ký với mã '{maDK}'.");
        }

        item.TrangThai = dto.TrangThai.Trim();
        await _repository.UpdateAsync(item);
        _logger.LogInformation("Đã cập nhật trạng thái đăng ký {MaDK} thành '{TrangThai}'", maDK, item.TrangThai);

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteDangKyAsync(int maDK)
    {
        var item = await _repository.GetByIdAsync(maDK);
        if (item == null)
        {
            return (false, $"Không tìm thấy đăng ký với mã '{maDK}'.");
        }

        await _repository.DeleteAsync(item);
        _logger.LogInformation("Đã xóa bản ghi đăng ký {MaDK}", maDK);

        return (true, null);
    }

    /// <summary>
    /// Làm giàu thông tin hiển thị bằng cách gọi song song sang 2 service
    /// Nếu một service bị gián đoạn, vẫn giữ nguyên dữ liệu cơ sở không làm crash ứng dụng (Graceful degradation)
    /// </summary>
    private async Task<DangKyResponseDto> EnrichDangKyDataAsync(DangKy dk)
    {
        string? hoTen = null;
        string? lop = null;
        string? tenDT = null;
        string? giangVien = null;

        try
        {
            var sv = await _sinhVienClient.GetSinhVienByIdAsync(dk.MaSV);
            if (sv != null)
            {
                hoTen = sv.HoTen;
                lop = sv.Lop;
            }
        }
        catch
        {
            // Bỏ qua lỗi enrichment để đảm bảo tính sẵn sàng
        }

        try
        {
            var dt = await _deTaiClient.GetDeTaiByIdAsync(dk.MaDT);
            if (dt != null)
            {
                tenDT = dt.TenDT;
                giangVien = dt.GiangVienHD;
            }
        }
        catch
        {
            // Bỏ qua lỗi enrichment
        }

        return new DangKyResponseDto
        {
            MaDK = dk.MaDK,
            MaSV = dk.MaSV,
            MaDT = dk.MaDT,
            NgayDangKy = dk.NgayDangKy,
            TrangThai = dk.TrangThai,
            HoTenSinhVien = hoTen,
            LopSinhVien = lop,
            TenDeTai = tenDT,
            GiangVienHD = giangVien
        };
    }
}
