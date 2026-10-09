using AuthService.Data;
using AuthService.DTOs;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services;

public interface IAuthService
{
    Task<(bool Success, string? ErrorMessage, AuthResponseDto? Result)> RegisterAsync(RegisterDto dto);
    Task<(bool Success, string? ErrorMessage, AuthResponseDto? Result)> LoginAsync(LoginDto dto);
}

public class AuthServiceImplementation : IAuthService
{
    private readonly AuthDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthServiceImplementation> _logger;

    public AuthServiceImplementation(AuthDbContext context, IConfiguration config, ILogger<AuthServiceImplementation> logger)
    {
        _context = context;
        _config = config;
        _logger = logger;
    }

    public async Task<(bool Success, string? ErrorMessage, AuthResponseDto? Result)> RegisterAsync(RegisterDto dto)
    {
        var username = dto.Username.Trim().ToLower();
        if (await _context.Users.AnyAsync(u => u.Username == username))
        {
            return (false, $"Tên tài khoản '{dto.Username}' đã tồn tại trong hệ thống.", null);
        }

        // Chuẩn hóa Role: chỉ cho phép "GiaoVien" hoặc "SinhVien"
        var role = dto.Role.Trim();
        if (role != "GiaoVien" && role != "SinhVien")
        {
            role = "SinhVien";
        }

        var user = new User
        {
            Username = username,
            PasswordHash = AuthDbContext.HashPassword(dto.Password),
            FullName = dto.FullName.Trim(),
            Role = role,
            ReferenceCode = dto.ReferenceCode?.Trim()
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Đăng ký thành công tài khoản: {Username} với vai trò {Role}", user.Username, user.Role);

        var token = GenerateJwtToken(user);
        var result = new AuthResponseDto
        {
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            ReferenceCode = user.ReferenceCode,
            Token = token
        };

        return (true, null, result);
    }

    public async Task<(bool Success, string? ErrorMessage, AuthResponseDto? Result)> LoginAsync(LoginDto dto)
    {
        var username = dto.Username.Trim().ToLower();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            return (false, "Tên đăng nhập hoặc mật khẩu không chính xác.", null);
        }

        var passwordHash = AuthDbContext.HashPassword(dto.Password);
        if (user.PasswordHash != passwordHash)
        {
            return (false, "Tên đăng nhập hoặc mật khẩu không chính xác.", null);
        }

        var token = GenerateJwtToken(user);
        var result = new AuthResponseDto
        {
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            ReferenceCode = user.ReferenceCode,
            Token = token
        };

        _logger.LogInformation("Người dùng {Username} ({Role}) đăng nhập thành công.", user.Username, user.Role);
        return (true, null, result);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _config["Jwt:Key"] ?? "SecretKeyQuangLyDoAnSOADemo2026123456789";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("FullName", user.FullName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("ReferenceCode", user.ReferenceCode ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "AuthService",
            audience: _config["Jwt:Audience"] ?? "QuanLyDoAnSOA",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
