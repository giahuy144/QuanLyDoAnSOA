using WebClient.Models;

namespace WebClient.Services;

public interface IApiClientService
{
    // SinhVien APIs
    Task<List<SinhVienViewModel>> GetSinhViensAsync();
    Task<SinhVienViewModel?> GetSinhVienByIdAsync(string maSV);
    Task<(bool Success, string? ErrorMessage)> CreateSinhVienAsync(SinhVienViewModel model);
    Task<(bool Success, string? ErrorMessage)> UpdateSinhVienAsync(string maSV, SinhVienViewModel model);
    Task<(bool Success, string? ErrorMessage)> DeleteSinhVienAsync(string maSV);

    // DeTai APIs
    Task<List<DeTaiViewModel>> GetDeTaisAsync();
    Task<DeTaiViewModel?> GetDeTaiByIdAsync(string maDT);
    Task<(bool Success, string? ErrorMessage)> CreateDeTaiAsync(DeTaiViewModel model);
    Task<(bool Success, string? ErrorMessage)> UpdateDeTaiAsync(string maDT, DeTaiViewModel model);
    Task<(bool Success, string? ErrorMessage)> DeleteDeTaiAsync(string maDT);

    // DangKy APIs
    Task<List<DangKyViewModel>> GetDangKysAsync();
    Task<(bool Success, string? ErrorMessage)> CreateDangKyAsync(string maSV, string maDT);
    Task<(bool Success, string? ErrorMessage)> DeleteDangKyAsync(int maDK);

    // Auth APIs (gọi sang AuthService - cổng 5004)
    Task<(bool Success, string? ErrorMessage, AuthResponseViewModel? Data)> LoginAsync(LoginViewModel model);
    Task<(bool Success, string? ErrorMessage, AuthResponseViewModel? Data)> RegisterAsync(RegisterViewModel model);
}
