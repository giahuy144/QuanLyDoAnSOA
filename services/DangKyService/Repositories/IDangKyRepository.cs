using DangKyService.Models;

namespace DangKyService.Repositories;

/// <summary>
/// Interface truy xuất CSDL cho Đăng ký
/// </summary>
public interface IDangKyRepository
{
    Task<IEnumerable<DangKy>> GetAllAsync();
    Task<DangKy?> GetByIdAsync(int maDK);
    Task<IEnumerable<DangKy>> GetByMaSVAsync(string maSV);
    Task<IEnumerable<DangKy>> GetByMaDTAsync(string maDT);
    Task<int> CountByMaDTAsync(string maDT);
    Task<bool> HasStudentRegisteredAsync(string maSV);
    Task<DangKy> AddAsync(DangKy dangKy);
    Task UpdateAsync(DangKy dangKy);
    Task DeleteAsync(DangKy dangKy);
}
