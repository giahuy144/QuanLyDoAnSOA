using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("USERS");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(20).IsRequired();
            entity.Property(e => e.ReferenceCode).HasMaxLength(50);
        });

        // Seed 2 tài khoản mẫu: 1 Giáo viên và 1 Sinh viên (Password: 123456)
        var passHash = HashPassword("123456");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "giaovien1",
                PasswordHash = passHash,
                FullName = "TS. Trần Văn Hùng",
                Role = "GiaoVien",
                ReferenceCode = "GV001"
            },
            new User
            {
                Id = 2,
                Username = "sinhvien1",
                PasswordHash = passHash,
                FullName = "Nguyễn Văn An",
                Role = "SinhVien",
                ReferenceCode = "SV001"
            }
        );
    }

    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
