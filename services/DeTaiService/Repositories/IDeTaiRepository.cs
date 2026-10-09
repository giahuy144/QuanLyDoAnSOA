using DeTaiService.Models;

namespace DeTaiService.Repositories;

/// <summary>
/// Interface trừu tượng hóa truy cập CSDL cho DeTai
/// </summary>
public interface IDeTaiRepository
{
    Task<IEnumerable<DeTai>> GetAllAsync();
    Task<DeTai?> GetByIdAsync(string maDT);
    Task<bool> ExistsAsync(string maDT);
    Task<DeTai> AddAsync(DeTai deTai);
    Task UpdateAsync(DeTai deTai);
    Task DeleteAsync(DeTai deTai);
}
