using SinhVienService.DTOs;
using SinhVienService.Models;
using SinhVienService.Repositories;

namespace SinhVienService.Services;

/// <summary>
/// Service triển khai logic nghiệp vụ cho Quản lý Sinh viên
/// </summary>
public class SinhVienAppService : ISinhVienService
{
    private readonly ISinhVienRepository _repository;
    private readonly ILogger<SinhVienAppService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public SinhVienAppService(
        ISinhVienRepository repository,
        ILogger<SinhVienAppService> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _repository = repository;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<IEnumerable<SinhVienResponseDto>> GetAllSinhViensAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(s => new SinhVienResponseDto
        {
            MaSV = s.MaSV,
            HoTen = s.HoTen,
            Lop = s.Lop,
            Email = s.Email,
            SoDienThoai = s.SoDienThoai
        });
    }

    public async Task<SinhVienResponseDto?> GetSinhVienByIdAsync(string maSV)
    {
        var sinhVien = await _repository.GetByIdAsync(maSV);
        if (sinhVien == null) return null;

        return new SinhVienResponseDto
        {
            MaSV = sinhVien.MaSV,
            HoTen = sinhVien.HoTen,
            Lop = sinhVien.Lop,
            Email = sinhVien.Email,
            SoDienThoai = sinhVien.SoDienThoai
        };
    }

    public async Task<(bool Success, string? ErrorMessage, SinhVienResponseDto? Result)> CreateSinhVienAsync(CreateSinhVienDto dto)
    {
        if (await _repository.ExistsAsync(dto.MaSV))
        {
            return (false, $"Mã sinh viên '{dto.MaSV}' đã tồn tại trong hệ thống.", null);
        }

        var entity = new SinhVien
        {
            MaSV = dto.MaSV.Trim().ToUpper(),
            HoTen = dto.HoTen.Trim(),
            Lop = dto.Lop.Trim(),
            Email = dto.Email.Trim(),
            SoDienThoai = dto.SoDienThoai.Trim()
        };

        var created = await _repository.AddAsync(entity);
        _logger.LogInformation("Đã thêm mới sinh viên: {MaSV} - {HoTen}", created.MaSV, created.HoTen);

        var result = new SinhVienResponseDto
        {
            MaSV = created.MaSV,
            HoTen = created.HoTen,
            Lop = created.Lop,
            Email = created.Email,
            SoDienThoai = created.SoDienThoai
        };

        return (true, null, result);
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateSinhVienAsync(string maSV, UpdateSinhVienDto dto)
    {
        var entity = await _repository.GetByIdAsync(maSV);
        if (entity == null)
        {
            return (false, $"Không tìm thấy sinh viên có mã '{maSV}'.");
        }

        entity.HoTen = dto.HoTen.Trim();
        entity.Lop = dto.Lop.Trim();
        entity.Email = dto.Email.Trim();
        entity.SoDienThoai = dto.SoDienThoai.Trim();

        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Đã cập nhật sinh viên: {MaSV}", maSV);

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage, bool IsConflict)> DeleteSinhVienAsync(string maSV)
    {
        var entity = await _repository.GetByIdAsync(maSV);
        if (entity == null)
        {
            return (false, $"Không tìm thấy sinh viên có mã '{maSV}'.", false);
        }

        // Kiểm tra ràng buộc SOA: Hỏi DangKyService xem sinh viên này có đồ án đang đăng ký không
        var dangKyServiceUrl = _configuration["Services:DangKyService"];
        if (!string.IsNullOrEmpty(dangKyServiceUrl))
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(3);
                var response = await client.GetAsync($"{dangKyServiceUrl}/api/dangky/sinhvien/{maSV}");
                
                if (response.IsSuccessStatusCode)
                {
                    // Nếu trả về 200 OK và có dữ liệu đăng ký
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content) && content != "[]" && content != "null")
                    {
                        return (false, $"Không thể xóa sinh viên '{maSV}' vì đang có đăng ký đồ án tốt nghiệp.", true);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Không thể kết nối đến DangKyService để kiểm tra ràng buộc trước khi xóa sinh viên.");
                // Trong trường hợp DangKyService offline hoặc chưa start, cho phép ghi log cảnh báo
            }
        }

        await _repository.DeleteAsync(entity);
        _logger.LogInformation("Đã xóa sinh viên: {MaSV}", maSV);

        return (true, null, false);
    }
}
