using System.ComponentModel.DataAnnotations;

namespace SinhVienService.DTOs;

/// <summary>
/// DTO trả về cho client/dịch vụ khác khi truy vấn thông tin sinh viên
/// </summary>
public class SinhVienResponseDto
{
    public string MaSV { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string Lop { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
}

/// <summary>
/// DTO tiếp nhận dữ liệu khi thêm mới một sinh viên
/// </summary>
public class CreateSinhVienDto
{
    [Required(ErrorMessage = "Mã sinh viên là bắt buộc")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Mã sinh viên phải từ 3 đến 20 ký tự")]
    public string MaSV { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên sinh viên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lớp là bắt buộc")]
    [StringLength(50, ErrorMessage = "Lớp tối đa 50 ký tự")]
    public string Lop { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
    public string SoDienThoai { get; set; } = string.Empty;
}

/// <summary>
/// DTO tiếp nhận dữ liệu khi cập nhật thông tin sinh viên (không cho sửa MaSV)
/// </summary>
public class UpdateSinhVienDto
{
    [Required(ErrorMessage = "Họ tên sinh viên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lớp là bắt buộc")]
    [StringLength(50, ErrorMessage = "Lớp tối đa 50 ký tự")]
    public string Lop { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
    public string SoDienThoai { get; set; } = string.Empty;
}
