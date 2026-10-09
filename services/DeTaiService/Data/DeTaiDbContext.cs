using Microsoft.EntityFrameworkCore;
using DeTaiService.Models;

namespace DeTaiService.Data;

/// <summary>
/// DbContext riêng biệt cho DeTaiService (Database-per-service SOA pattern)
/// </summary>
public class DeTaiDbContext : DbContext
{
    public DeTaiDbContext(DbContextOptions<DeTaiDbContext> options) : base(options)
    {
    }

    public DbSet<DeTai> DeTais => Set<DeTai>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DeTai>(entity =>
        {
            entity.ToTable("DETAI");
            entity.HasKey(e => e.MaDT);
            entity.Property(e => e.MaDT).HasMaxLength(20).IsRequired();
            entity.Property(e => e.TenDT).HasMaxLength(200).IsRequired();
            entity.Property(e => e.MoTa).HasMaxLength(1000);
            entity.Property(e => e.GiangVienHD).HasMaxLength(100).IsRequired();
            entity.Property(e => e.SoLuongToiDa).IsRequired();
        });

        // Seed dữ liệu mẫu đề tài
        modelBuilder.Entity<DeTai>().HasData(
            new DeTai
            {
                MaDT = "DT01",
                TenDT = "Hệ thống Quản lý Đồ án theo Kiến trúc SOA",
                MoTa = "Xây dựng hệ thống phân tán chia theo các service độc lập sử dụng .NET 8 Web API và Microservices pattern.",
                GiangVienHD = "TS. Trần Văn Hùng",
                SoLuongToiDa = 2
            },
            new DeTai
            {
                MaDT = "DT02",
                TenDT = "Ứng dụng AI Nhận diện Khuôn mặt trong Điểm danh",
                MoTa = "Tích hợp mô hình Deep Learning xử lý ảnh điểm danh sinh viên tự động theo thời gian thực.",
                GiangVienHD = "ThS. Lê Thị Mai",
                SoLuongToiDa = 2
            },
            new DeTai
            {
                MaDT = "DT03",
                TenDT = "Nền tảng Thương mại Điện tử hỗ trợ Thanh toán Trực tuyến",
                MoTa = "Phát triển sàn giao dịch tích hợp cổng thanh toán VNPay, ZaloPay và quản lý kho hàng.",
                GiangVienHD = "TS. Nguyễn Quốc Bảo",
                SoLuongToiDa = 3
            }
        );
    }
}
