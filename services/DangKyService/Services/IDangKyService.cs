using DangKyService.DTOs;

namespace DangKyService.Services;

/// <summary>
/// Interface nghiệp vụ cho DangKyService
/// </summary>
public interface IDangKyService
{
    Task<IEnumerable<DangKyResponseDto>> GetAllDangKysAsync();
    Task<DangKyResponseDto?> GetDangKyByIdAsync(int maDK);
    Task<IEnumerable<DangKyResponseDto>> GetDangKysByMaSVAsync(string maSV);
    Task<IEnumerable<DangKyResponseDto>> GetDangKysByMaDTAsync(string maDT);
    Task<(bool Success, int StatusCode, string? ErrorMessage, DangKyResponseDto? Result)> CreateDangKyAsync(CreateDangKyDto dto);
    Task<(bool Success, string? ErrorMessage)> UpdateTrangThaiAsync(int maDK, UpdateDangKyDto dto);
    Task<(bool Success, string? ErrorMessage)> DeleteDangKyAsync(int maDK);
}
