using Microsoft.EntityFrameworkCore;
using DeTaiService.Data;
using DeTaiService.Models;

namespace DeTaiService.Repositories;

public class DeTaiRepository : IDeTaiRepository
{
    private readonly DeTaiDbContext _context;

    public DeTaiRepository(DeTaiDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DeTai>> GetAllAsync()
    {
        return await _context.DeTais.AsNoTracking().ToListAsync();
    }

    public async Task<DeTai?> GetByIdAsync(string maDT)
    {
        return await _context.DeTais.FindAsync(maDT);
    }

    public async Task<bool> ExistsAsync(string maDT)
    {
        return await _context.DeTais.AnyAsync(d => d.MaDT == maDT);
    }

    public async Task<DeTai> AddAsync(DeTai deTai)
    {
        await _context.DeTais.AddAsync(deTai);
        await _context.SaveChangesAsync();
        return deTai;
    }

    public async Task UpdateAsync(DeTai deTai)
    {
        _context.DeTais.Update(deTai);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(DeTai deTai)
    {
        _context.DeTais.Remove(deTai);
        await _context.SaveChangesAsync();
    }
}
