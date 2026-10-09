using Microsoft.EntityFrameworkCore;
using DangKyService.Models;

namespace DangKyService.Data;

/// <summary>
/// DbContext riêng của DangKyService (Database-per-service pattern)
/// </summary>
public class DangKyDbContext : DbContext
{
    public DangKyDbContext(DbContextOptions<DangKyDbContext> options) : base(options)
    {
    }

    public DbSet<DangKy> DangKys => Set<DangKy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DangKy>(entity =>
        {
            entity.ToTable("DANGKY");
            entity.HasKey(e => e.MaDK);
            entity.Property(e => e.MaDK).ValueGeneratedOnAdd();
            entity.Property(e => e.MaSV).HasMaxLength(20).IsRequired();
            entity.Property(e => e.MaDT).HasMaxLength(20).IsRequired();
            entity.Property(e => e.NgayDangKy).IsRequired();
            entity.Property(e => e.TrangThai).HasMaxLength(50).IsRequired();

            // Ràng buộc logic mức DB: Một sinh viên chỉ được phép có 1 bản ghi đăng ký đang hiệu lực
            entity.HasIndex(e => e.MaSV);
            entity.HasIndex(e => e.MaDT);
        });

        // Seed 1 bản ghi mẫu ban đầu
        modelBuilder.Entity<DangKy>().HasData(
            new DangKy
            {
                MaDK = 1,
                MaSV = "SV001",
                MaDT = "DT01",
                NgayDangKy = new DateTime(2026, 10, 1, 8, 30, 0, DateTimeKind.Utc),
                TrangThai = "DaDangKy"
            }
        );
    }
}
