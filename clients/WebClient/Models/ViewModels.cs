using System.ComponentModel.DataAnnotations;

namespace WebClient.Models;

public class SinhVienViewModel
{
    [Required(ErrorMessage = "Mã sinh viên là bắt buộc")]
    [Display(Name = "Mã SV")]
    public string MaSV { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên sinh viên là bắt buộc")]
    [Display(Name = "Họ và tên")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lớp là bắt buộc")]
    [Display(Name = "Lớp")]
    public string Lop { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Display(Name = "Số điện thoại")]
    public string SoDienThoai { get; set; } = string.Empty;
}

public class DeTaiViewModel
{
    [Required(ErrorMessage = "Mã đề tài là bắt buộc")]
    [Display(Name = "Mã đề tài")]
    public string MaDT { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên đề tài là bắt buộc")]
    [Display(Name = "Tên đề tài")]
    public string TenDT { get; set; } = string.Empty;

    [Display(Name = "Mô tả đề tài")]
    public string MoTa { get; set; } = string.Empty;

    [Required(ErrorMessage = "Giảng viên hướng dẫn là bắt buộc")]
    [Display(Name = "Giảng viên HD")]
    public string GiangVienHD { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số lượng tối đa là bắt buộc")]
    [Range(1, 10, ErrorMessage = "Số lượng từ 1 đến 10")]
    [Display(Name = "Số lượng SV tối đa")]
    public int SoLuongToiDa { get; set; } = 2;
}

public class DangKyViewModel
{
    public int MaDK { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn sinh viên")]
    [Display(Name = "Sinh viên")]
    public string MaSV { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn đề tài")]
    [Display(Name = "Đề tài")]
    public string MaDT { get; set; } = string.Empty;

    [Display(Name = "Ngày đăng ký")]
    public DateTime NgayDangKy { get; set; }

    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = string.Empty;

    // Chi tiết làm giàu
    public string? HoTenSinhVien { get; set; }
    public string? LopSinhVien { get; set; }
    public string? TenDeTai { get; set; }
    public string? GiangVienHD { get; set; }
}

public class DangKyCreateViewModel
{
    [Required(ErrorMessage = "Vui lòng chọn sinh viên")]
    [Display(Name = "Sinh viên")]
    public string MaSV { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn đề tài")]
    [Display(Name = "Đề tài")]
    public string MaDT { get; set; } = string.Empty;

    public List<SinhVienViewModel> SinhViens { get; set; } = new();
    public List<DeTaiViewModel> DeTais { get; set; } = new();

    // Sinh viên đăng nhập sẽ bị khóa ô chọn sinh viên (chỉ được đăng ký cho chính mình)
    public bool KhoaChonSinhVien { get; set; }
}

#region Tài khoản (Authentication)

/// <summary>
/// Kết quả trả về từ AuthService sau khi đăng nhập / đăng ký thành công.
/// Tên thuộc tính khớp với AuthResponseDto bên AuthService (JSON camelCase).
/// </summary>
public class AuthResponseViewModel
{
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;         // "GiaoVien" | "SinhVien"
    public string? ReferenceCode { get; set; }               // Mã GV hoặc Mã SV liên kết
    public string Token { get; set; } = string.Empty;        // JWT do AuthService ký
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    [Display(Name = "Tên đăng nhập")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Ghi nhớ đăng nhập")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3 đến 50 ký tự")]
    [Display(Name = "Tên đăng nhập")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ và tên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn vai trò")]
    [Display(Name = "Vai trò")]
    public string Role { get; set; } = "SinhVien";   // "GiaoVien" | "SinhVien"

    [Display(Name = "Mã Giảng viên / Mã Sinh viên (nếu có)")]
    public string? ReferenceCode { get; set; }
}

#endregion
