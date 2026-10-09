using DeTaiService.DTOs;
using DeTaiService.Models;
using DeTaiService.Repositories;

namespace DeTaiService.Services;

/// <summary>
/// Service triển khai nghiệp vụ cho Quản lý Đề tài
/// </summary>
public class DeTaiAppService : IDeTaiService
{
    private readonly IDeTaiRepository _repository;
    private readonly ILogger<DeTaiAppService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public DeTaiAppService(
        IDeTaiRepository repository,
        ILogger<DeTaiAppService> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _repository = repository;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<IEnumerable<DeTaiResponseDto>> GetAllDeTaisAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(d => new DeTaiResponseDto
        {
            MaDT = d.MaDT,
            TenDT = d.TenDT,
            MoTa = d.MoTa,
            GiangVienHD = d.GiangVienHD,
            SoLuongToiDa = d.SoLuongToiDa
        });
    }

    public async Task<DeTaiResponseDto?> GetDeTaiByIdAsync(string maDT)
    {
        var d = await _repository.GetByIdAsync(maDT);
        if (d == null) return null;

        return new DeTaiResponseDto
        {
            MaDT = d.MaDT,
            TenDT = d.TenDT,
            MoTa = d.MoTa,
            GiangVienHD = d.GiangVienHD,
            SoLuongToiDa = d.SoLuongToiDa
        };
    }

    public async Task<(bool Success, string? ErrorMessage, DeTaiResponseDto? Result)> CreateDeTaiAsync(CreateDeTaiDto dto)
    {
        if (await _repository.ExistsAsync(dto.MaDT))
        {
            return (false, $"Mã đề tài '{dto.MaDT}' đã tồn tại trong hệ thống.", null);
        }

        var entity = new DeTai
        {
            MaDT = dto.MaDT.Trim().ToUpper(),
            TenDT = dto.TenDT.Trim(),
            MoTa = dto.MoTa?.Trim() ?? string.Empty,
            GiangVienHD = dto.GiangVienHD.Trim(),
            SoLuongToiDa = dto.SoLuongToiDa
        };

        var created = await _repository.AddAsync(entity);
        _logger.LogInformation("Đã thêm mới đề tài: {MaDT} - {TenDT}", created.MaDT, created.TenDT);

        var result = new DeTaiResponseDto
        {
            MaDT = created.MaDT,
            TenDT = created.TenDT,
            MoTa = created.MoTa,
            GiangVienHD = created.GiangVienHD,
            SoLuongToiDa = created.SoLuongToiDa
        };

        return (true, null, result);
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateDeTaiAsync(string maDT, UpdateDeTaiDto dto)
    {
        var entity = await _repository.GetByIdAsync(maDT);
        if (entity == null)
        {
            return (false, $"Không tìm thấy đề tài có mã '{maDT}'.");
        }

        entity.TenDT = dto.TenDT.Trim();
        entity.MoTa = dto.MoTa?.Trim() ?? string.Empty;
        entity.GiangVienHD = dto.GiangVienHD.Trim();
        entity.SoLuongToiDa = dto.SoLuongToiDa;

        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Đã cập nhật đề tài: {MaDT}", maDT);

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage, bool IsConflict)> DeleteDeTaiAsync(string maDT)
    {
        var entity = await _repository.GetByIdAsync(maDT);
        if (entity == null)
        {
            return (false, $"Không tìm thấy đề tài có mã '{maDT}'.", false);
        }

        // Kiểm tra ràng buộc SOA: Hỏi DangKyService xem đề tài này có sinh viên đang đăng ký không
        var dangKyServiceUrl = _configuration["Services:DangKyService"];
        if (!string.IsNullOrEmpty(dangKyServiceUrl))
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(3);
                var response = await client.GetAsync($"{dangKyServiceUrl}/api/dangky/detai/{maDT}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content) && content != "[]" && content != "null")
                    {
                        return (false, $"Không thể xóa đề tài '{maDT}' vì đang có sinh viên đăng ký.", true);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Không thể kết nối đến DangKyService để kiểm tra ràng buộc trước khi xóa đề tài.");
            }
        }

        await _repository.DeleteAsync(entity);
        _logger.LogInformation("Đã xóa đề tài: {MaDT}", maDT);

        return (true, null, false);
    }
}
