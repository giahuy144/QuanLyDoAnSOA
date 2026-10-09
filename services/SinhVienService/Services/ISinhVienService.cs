using SinhVienService.DTOs;

namespace SinhVienService.Services;

/// <summary>
/// Interface nghiệp vụ cho SinhVien
/// </summary>
public interface ISinhVienService
{
    Task<IEnumerable<SinhVienResponseDto>> GetAllSinhViensAsync();
    Task<SinhVienResponseDto?> GetSinhVienByIdAsync(string maSV);
    Task<(bool Success, string? ErrorMessage, SinhVienResponseDto? Result)> CreateSinhVienAsync(CreateSinhVienDto dto);
    Task<(bool Success, string? ErrorMessage)> UpdateSinhVienAsync(string maSV, UpdateSinhVienDto dto);
    Task<(bool Success, string? ErrorMessage, bool IsConflict)> DeleteSinhVienAsync(string maSV);
}
