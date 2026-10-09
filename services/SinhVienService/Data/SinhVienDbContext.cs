using Microsoft.EntityFrameworkCore;
using SinhVienService.Models;

namespace SinhVienService.Data;

/// <summary>
/// DbContext riêng biệt của SinhVienService theo nguyên tắc Database-per-service trong kiến trúc SOA
/// </summary>
public class SinhVienDbContext : DbContext
{
    public SinhVienDbContext(DbContextOptions<SinhVienDbContext> options) : base(options)
    {
    }

    public DbSet<SinhVien> SinhViens => Set<SinhVien>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.ToTable("SINHVIEN");
            entity.HasKey(e => e.MaSV);
            entity.Property(e => e.MaSV).HasMaxLength(20).IsRequired();
            entity.Property(e => e.HoTen).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Lop).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.SoDienThoai).HasMaxLength(15).IsRequired();
        });

        // Seed data ban đầu để test ngay
        modelBuilder.Entity<SinhVien>().HasData(
            new SinhVien { MaSV = "SV001", HoTen = "Nguyễn Văn An", Lop = "CNTT1", Email = "an.nv@university.edu.vn", SoDienThoai = "0901234567" },
            new SinhVien { MaSV = "SV002", HoTen = "Trần Thị Bình", Lop = "CNTT1", Email = "binh.tt@university.edu.vn", SoDienThoai = "0902345678" },
            new SinhVien { MaSV = "SV003", HoTen = "Lê Hoàng Cường", Lop = "CNTT2", Email = "cuong.lh@university.edu.vn", SoDienThoai = "0903456789" },
            new SinhVien { MaSV = "SV004", HoTen = "Phạm Minh Đức", Lop = "CNTT2", Email = "duc.pm@university.edu.vn", SoDienThoai = "0904567890" }
        );
    }
}
