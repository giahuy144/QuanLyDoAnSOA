using DangKyService.DTOs;

namespace DangKyService.Clients;

/// <summary>
/// Typed HttpClient giao tiếp với SinhVienService (cổng 5001)
/// </summary>
public interface ISinhVienClient
{
    Task<SinhVienDto?> GetSinhVienByIdAsync(string maSV);
}

public class SinhVienClient : ISinhVienClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SinhVienClient> _logger;

    public SinhVienClient(HttpClient httpClient, ILogger<SinhVienClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<SinhVienDto?> GetSinhVienByIdAsync(string maSV)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/sinhvien/{maSV}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SinhVienDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Lỗi HTTP khi gọi SinhVienService cho MaSV: {MaSV}", maSV);
            throw;
        }
    }
}
