using DangKyService.DTOs;

namespace DangKyService.Clients;

/// <summary>
/// Typed HttpClient giao tiếp với DeTaiService (cổng 5002)
/// </summary>
public interface IDeTaiClient
{
    Task<DeTaiDto?> GetDeTaiByIdAsync(string maDT);
}

public class DeTaiClient : IDeTaiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DeTaiClient> _logger;

    public DeTaiClient(HttpClient httpClient, ILogger<DeTaiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DeTaiDto?> GetDeTaiByIdAsync(string maDT)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/detai/{maDT}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DeTaiDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Lỗi HTTP khi gọi DeTaiService cho MaDT: {MaDT}", maDT);
            throw;
        }
    }
}
