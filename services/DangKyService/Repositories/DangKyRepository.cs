using Microsoft.EntityFrameworkCore;
using DangKyService.Data;
using DangKyService.Models;

namespace DangKyService.Repositories;

public class DangKyRepository : IDangKyRepository
{
    private readonly DangKyDbContext _context;

    public DangKyRepository(DangKyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DangKy>> GetAllAsync()
    {
        return await _context.DangKys.AsNoTracking().ToListAsync();
    }

    public async Task<DangKy?> GetByIdAsync(int maDK)
    {
        return await _context.DangKys.FindAsync(maDK);
    }

    public async Task<IEnumerable<DangKy>> GetByMaSVAsync(string maSV)
    {
        return await _context.DangKys
            .AsNoTracking()
            .Where(d => d.MaSV == maSV)
            .ToListAsync();
    }

    public async Task<IEnumerable<DangKy>> GetByMaDTAsync(string maDT)
    {
        return await _context.DangKys
            .AsNoTracking()
            .Where(d => d.MaDT == maDT)
            .ToListAsync();
    }

    public async Task<int> CountByMaDTAsync(string maDT)
    {
        return await _context.DangKys
            .CountAsync(d => d.MaDT == maDT && d.TrangThai != "DaHuy");
    }

    public async Task<bool> HasStudentRegisteredAsync(string maSV)
    {
        return await _context.DangKys
            .AnyAsync(d => d.MaSV == maSV && d.TrangThai != "DaHuy");
    }

    public async Task<DangKy> AddAsync(DangKy dangKy)
    {
        await _context.DangKys.AddAsync(dangKy);
        await _context.SaveChangesAsync();
        return dangKy;
    }

    public async Task UpdateAsync(DangKy dangKy)
    {
        _context.DangKys.Update(dangKy);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(DangKy dangKy)
    {
        _context.DangKys.Remove(dangKy);
        await _context.SaveChangesAsync();
    }
}
