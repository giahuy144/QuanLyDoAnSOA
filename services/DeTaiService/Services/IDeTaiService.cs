using DeTaiService.DTOs;

namespace DeTaiService.Services;

/// <summary>
/// Interface nghiệp vụ cho DeTaiService
/// </summary>
public interface IDeTaiService
{
    Task<IEnumerable<DeTaiResponseDto>> GetAllDeTaisAsync();
    Task<DeTaiResponseDto?> GetDeTaiByIdAsync(string maDT);
    Task<(bool Success, string? ErrorMessage, DeTaiResponseDto? Result)> CreateDeTaiAsync(CreateDeTaiDto dto);
    Task<(bool Success, string? ErrorMessage)> UpdateDeTaiAsync(string maDT, UpdateDeTaiDto dto);
    Task<(bool Success, string? ErrorMessage, bool IsConflict)> DeleteDeTaiAsync(string maDT);
}
