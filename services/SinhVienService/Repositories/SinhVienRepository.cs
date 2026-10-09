using Microsoft.EntityFrameworkCore;
using SinhVienService.Data;
using SinhVienService.Models;

namespace SinhVienService.Repositories;

public class SinhVienRepository : ISinhVienRepository
{
    private readonly SinhVienDbContext _context;

    public SinhVienRepository(SinhVienDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SinhVien>> GetAllAsync()
    {
        return await _context.SinhViens.AsNoTracking().ToListAsync();
    }

    public async Task<SinhVien?> GetByIdAsync(string maSV)
    {
        return await _context.SinhViens.FindAsync(maSV);
    }

    public async Task<bool> ExistsAsync(string maSV)
    {
        return await _context.SinhViens.AnyAsync(s => s.MaSV == maSV);
    }

    public async Task<SinhVien> AddAsync(SinhVien sinhVien)
    {
        await _context.SinhViens.AddAsync(sinhVien);
        await _context.SaveChangesAsync();
        return sinhVien;
    }

    public async Task UpdateAsync(SinhVien sinhVien)
    {
        _context.SinhViens.Update(sinhVien);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(SinhVien sinhVien)
    {
        _context.SinhViens.Remove(sinhVien);
        await _context.SaveChangesAsync();
    }
}
