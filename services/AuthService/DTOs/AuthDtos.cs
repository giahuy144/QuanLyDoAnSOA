using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3 đến 50 ký tự")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(100, MinimumLength = 4, ErrorMessage = "Mật khẩu ít nhất 4 ký tự")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ và tên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Cho phép chọn "GiaoVien" hoặc "SinhVien"
    /// </summary>
    [Required(ErrorMessage = "Vui lòng chọn vai trò (GiaoVien hoặc SinhVien)")]
    public string Role { get; set; } = "SinhVien";

    /// <summary>
    /// Mã sinh viên liên kết (nếu chọn quyền SinhVien)
    /// </summary>
    public string? ReferenceCode { get; set; }
}

public class LoginDto
{
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? ReferenceCode { get; set; }
    public string Token { get; set; } = string.Empty;
}
