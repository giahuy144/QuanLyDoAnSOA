using SinhVienService.Models;

namespace SinhVienService.Repositories;

/// <summary>
/// Interface trừu tượng hóa tầng truy xuất CSDL cho SinhVien
/// Giúp phân tách tầng nghiệp vụ (Service) khỏi tầng cơ sở dữ liệu (DbContext/Repository)
/// </summary>
public interface ISinhVienRepository
{
    Task<IEnumerable<SinhVien>> GetAllAsync();
    Task<SinhVien?> GetByIdAsync(string maSV);
    Task<bool> ExistsAsync(string maSV);
    Task<SinhVien> AddAsync(SinhVien sinhVien);
    Task UpdateAsync(SinhVien sinhVien);
    Task DeleteAsync(SinhVien sinhVien);
}
