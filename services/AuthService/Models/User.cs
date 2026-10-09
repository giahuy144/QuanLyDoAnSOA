namespace AuthService.Models;

/// <summary>
/// Đại diện cho tài khoản người dùng trong hệ thống SOA
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    /// <summary>
    /// Vai trò: "GiaoVien" hoặc "SinhVien"
    /// </summary>
    public string Role { get; set; } = string.Empty;
    /// <summary>
    /// Mã liên kết nếu là sinh viên (ví dụ: SV001) hoặc giảng viên
    /// </summary>
    public string? ReferenceCode { get; set; }
}
