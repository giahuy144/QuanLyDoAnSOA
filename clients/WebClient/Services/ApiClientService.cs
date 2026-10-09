using System.Text.Json;
using WebClient.Models;

namespace WebClient.Services;

public class ApiClientService : IApiClientService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<ApiClientService> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ApiClientService(HttpClient httpClient, IConfiguration config, ILogger<ApiClientService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    private string SinhVienUrl => _config["Services:SinhVienService"] ?? "http://localhost:5001";
    private string DeTaiUrl => _config["Services:DeTaiService"] ?? "http://localhost:5002";
    private string DangKyUrl => _config["Services:DangKyService"] ?? "http://localhost:5003";

    #region SinhVien
    public async Task<List<SinhVienViewModel>> GetSinhViensAsync()
    {
        try
        {
            var res = await _httpClient.GetAsync($"{SinhVienUrl}/api/sinhvien");
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<SinhVienViewModel>>(content, _jsonOptions) ?? new();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gọi SinhVienService.");
        }
        return new();
    }

    public async Task<SinhVienViewModel?> GetSinhVienByIdAsync(string maSV)
    {
        try
        {
            var res = await _httpClient.GetAsync($"{SinhVienUrl}/api/sinhvien/{maSV}");
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<SinhVienViewModel>(content, _jsonOptions);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gọi SinhVienService GetById.");
        }
        return null;
    }

    public async Task<(bool Success, string? ErrorMessage)> CreateSinhVienAsync(SinhVienViewModel model)
    {
        try
        {
            var res = await _httpClient.PostAsJsonAsync($"{SinhVienUrl}/api/sinhvien", model);
            if (res.IsSuccessStatusCode) return (true, null);

            var error = await res.Content.ReadAsStringAsync();
            return (false, ParseErrorMessage(error));
        }
        catch (Exception ex)
        {
            return (false, $"Không thể kết nối đến SinhVienService: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateSinhVienAsync(string maSV, SinhVienViewModel model)
    {
        try
        {
            var res = await _httpClient.PutAsJsonAsync($"{SinhVienUrl}/api/sinhvien/{maSV}", model);
            if (res.IsSuccessStatusCode) return (true, null);

            var error = await res.Content.ReadAsStringAsync();
            return (false, ParseErrorMessage(error));
        }
        catch (Exception ex)
        {
            return (false, $"Không thể kết nối đến SinhVienService: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteSinhVienAsync(string maSV)
    {
        try
        {
            var res = await _httpClient.DeleteAsync($"{SinhVienUrl}/api/sinhvien/{maSV}");
            if (res.IsSuccessStatusCode) return (true, null);

            var error = await res.Content.ReadAsStringAsync();
            return (false, ParseErrorMessage(error));
        }
        catch (Exception ex)
        {
            return (false, $"Không thể kết nối đến SinhVienService: {ex.Message}");
        }
    }
    #endregion

    #region DeTai
    public async Task<List<DeTaiViewModel>> GetDeTaisAsync()
    {
        try
        {
            var res = await _httpClient.GetAsync($"{DeTaiUrl}/api/detai");
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<DeTaiViewModel>>(content, _jsonOptions) ?? new();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gọi DeTaiService.");
        }
        return new();
    }

    public async Task<DeTaiViewModel?> GetDeTaiByIdAsync(string maDT)
    {
        try
        {
            var res = await _httpClient.GetAsync($"{DeTaiUrl}/api/detai/{maDT}");
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DeTaiViewModel>(content, _jsonOptions);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gọi DeTaiService GetById.");
        }
        return null;
    }

    public async Task<(bool Success, string? ErrorMessage)> CreateDeTaiAsync(DeTaiViewModel model)
    {
        try
        {
            var res = await _httpClient.PostAsJsonAsync($"{DeTaiUrl}/api/detai", model);
            if (res.IsSuccessStatusCode) return (true, null);

            var error = await res.Content.ReadAsStringAsync();
            return (false, ParseErrorMessage(error));
        }
        catch (Exception ex)
        {
            return (false, $"Không thể kết nối đến DeTaiService: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateDeTaiAsync(string maDT, DeTaiViewModel model)
    {
        try
        {
            var res = await _httpClient.PutAsJsonAsync($"{DeTaiUrl}/api/detai/{maDT}", model);
            if (res.IsSuccessStatusCode) return (true, null);

            var error = await res.Content.ReadAsStringAsync();
            return (false, ParseErrorMessage(error));
        }
        catch (Exception ex)
        {
            return (false, $"Không thể kết nối đến DeTaiService: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteDeTaiAsync(string maDT)
    {
        try
        {
            var res = await _httpClient.DeleteAsync($"{DeTaiUrl}/api/detai/{maDT}");
            if (res.IsSuccessStatusCode) return (true, null);

            var error = await res.Content.ReadAsStringAsync();
            return (false, ParseErrorMessage(error));
        }
        catch (Exception ex)
        {
            return (false, $"Không thể kết nối đến DeTaiService: {ex.Message}");
        }
    }
    #endregion

    #region DangKy
    public async Task<List<DangKyViewModel>> GetDangKysAsync()
    {
        try
        {
            var res = await _httpClient.GetAsync($"{DangKyUrl}/api/dangky");
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<DangKyViewModel>>(content, _jsonOptions) ?? new();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gọi DangKyService.");
        }
        return new();
    }

    public async Task<(bool Success, string? ErrorMessage)> CreateDangKyAsync(string maSV, string maDT)
    {
        try
        {
            var body = new { maSV, maDT };
            var res = await _httpClient.PostAsJsonAsync($"{DangKyUrl}/api/dangky", body);
            if (res.IsSuccessStatusCode) return (true, null);

            var error = await res.Content.ReadAsStringAsync();
            return (false, ParseErrorMessage(error));
        }
        catch (Exception ex)
        {
            return (false, $"Không thể kết nối đến DangKyService: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteDangKyAsync(int maDK)
    {
        try
        {
            var res = await _httpClient.DeleteAsync($"{DangKyUrl}/api/dangky/{maDK}");
            if (res.IsSuccessStatusCode) return (true, null);

            var error = await res.Content.ReadAsStringAsync();
            return (false, ParseErrorMessage(error));
        }
        catch (Exception ex)
        {
            return (false, $"Không thể kết nối đến DangKyService: {ex.Message}");
        }
    }
    #endregion

    private static string ParseErrorMessage(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("message", out var msgElement))
            {
                return msgElement.GetString() ?? json;
            }
            if (doc.RootElement.TryGetProperty("detail", out var detailElement))
            {
                return detailElement.GetString() ?? json;
            }
        }
        catch
        {
            // Không phải json hợp lệ
        }
        return json;
    }
}
