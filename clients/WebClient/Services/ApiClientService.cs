using System.Net.Http.Headers;
using System.Text.Json;
using WebClient.Models;

namespace WebClient.Services;

public class ApiClientService : IApiClientService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<ApiClientService> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ApiClientService(HttpClient httpClient, IConfiguration config, ILogger<ApiClientService> logger,
                            IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;

        // Truyền JWT do AuthService cấp vào mọi request (SOA: token propagation).
        // Mỗi lần IHttpClientFactory tạo ApiClientService sẽ cấp một HttpClient mới,
        // nên việc gán DefaultRequestHeaders tại đây là an toàn.
        var token = httpContextAccessor.HttpContext?.User?.FindFirst("JwtToken")?.Value
                    ?? httpContextAccessor.HttpContext?.Session.GetString("JwtToken");

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private string SinhVienUrl => _config["Services:SinhVienService"] ?? "http://localhost:5001";
    private string DeTaiUrl => _config["Services:DeTaiService"] ?? "http://localhost:5002";
    private string DangKyUrl => _config["Services:DangKyService"] ?? "http://localhost:5003";
    private string AuthUrl => _config["Services:AuthService"] ?? "http://localhost:5004";

    /// <summary>Ghi log khi service trả về mã lỗi, giúp dễ debug khi dữ liệu trống.</summary>
    private void LogBadResponse(string serviceName, HttpResponseMessage res, string body)
    {
        _logger.LogWarning("{Service} trả về {Status}: {Body}",
            serviceName, (int)res.StatusCode, body.Length > 200 ? body[..200] : body);
    }

    #region SinhVien
    public async Task<List<SinhVienViewModel>> GetSinhViensAsync()
    {
        try
        {
            var res = await _httpClient.GetAsync($"{SinhVienUrl}/api/sinhvien");
            var content = await res.Content.ReadAsStringAsync();
            if (res.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<List<SinhVienViewModel>>(content, _jsonOptions) ?? new();
            }
            LogBadResponse("SinhVienService", res, content);
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
            var content = await res.Content.ReadAsStringAsync();
            if (res.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<List<DeTaiViewModel>>(content, _jsonOptions) ?? new();
            }
            LogBadResponse("DeTaiService", res, content);
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
            var content = await res.Content.ReadAsStringAsync();
            if (res.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<List<DangKyViewModel>>(content, _jsonOptions) ?? new();
            }
            LogBadResponse("DangKyService", res, content);
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

    #region Auth (AuthService - cổng 5004)
    public async Task<(bool Success, string? ErrorMessage, AuthResponseViewModel? Data)> LoginAsync(LoginViewModel model)
    {
        try
        {
            var body = new { username = model.Username, password = model.Password };
            var res = await _httpClient.PostAsJsonAsync($"{AuthUrl}/api/auth/login", body);
            var content = await res.Content.ReadAsStringAsync();

            if (res.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<AuthResponseViewModel>(content, _jsonOptions);
                return (true, null, data);
            }

            if ((int)res.StatusCode == 401)
            {
                return (false, "Sai tên đăng nhập hoặc mật khẩu.", null);
            }
            return (false, ParseErrorMessage(content), null);
        }
        catch (Exception ex)
        {
            return (false, $"Không thể kết nối đến AuthService (cổng 5004): {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string? ErrorMessage, AuthResponseViewModel? Data)> RegisterAsync(RegisterViewModel model)
    {
        try
        {
            var body = new
            {
                username = model.Username,
                password = model.Password,
                fullName = model.FullName,
                role = model.Role,
                referenceCode = model.ReferenceCode
            };

            var res = await _httpClient.PostAsJsonAsync($"{AuthUrl}/api/auth/register", body);
            var content = await res.Content.ReadAsStringAsync();

            if (res.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<AuthResponseViewModel>(content, _jsonOptions);
                return (true, null, data);
            }

            if ((int)res.StatusCode == 409)
            {
                return (false, "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác.", null);
            }
            return (false, ParseErrorMessage(content), null);
        }
        catch (Exception ex)
        {
            return (false, $"Không thể kết nối đến AuthService (cổng 5004): {ex.Message}", null);
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
