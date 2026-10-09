using System.ComponentModel.DataAnnotations;

namespace DeTaiService.DTOs;

/// <summary>
/// DTO trả về thông tin đề tài cho client và các service khác
/// </summary>
public class DeTaiResponseDto
{
    public string MaDT { get; set; } = string.Empty;
    public string TenDT { get; set; } = string.Empty;
    public string MoTa { get; set; } = string.Empty;
    public string GiangVienHD { get; set; } = string.Empty;
    public int SoLuongToiDa { get; set; }
}

/// <summary>
/// DTO tiếp nhận dữ liệu khi thêm mới đề tài
/// </summary>
public class CreateDeTaiDto
{
    [Required(ErrorMessage = "Mã đề tài là bắt buộc")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Mã đề tài phải từ 3 đến 20 ký tự")]
    public string MaDT { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên đề tài là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tên đề tài tối đa 200 ký tự")]
    public string TenDT { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
    public string MoTa { get; set; } = string.Empty;

    [Required(ErrorMessage = "Giảng viên hướng dẫn là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên giảng viên tối đa 100 ký tự")]
    public string GiangVienHD { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Số lượng sinh viên tối đa phải từ 1 đến 10")]
    public int SoLuongToiDa { get; set; } = 2;
}

/// <summary>
/// DTO tiếp nhận dữ liệu khi cập nhật đề tài (không cho sửa MaDT)
/// </summary>
public class UpdateDeTaiDto
{
    [Required(ErrorMessage = "Tên đề tài là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tên đề tài tối đa 200 ký tự")]
    public string TenDT { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
    public string MoTa { get; set; } = string.Empty;

    [Required(ErrorMessage = "Giảng viên hướng dẫn là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên giảng viên tối đa 100 ký tự")]
    public string GiangVienHD { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Số lượng sinh viên tối đa phải từ 1 đến 10")]
    public int SoLuongToiDa { get; set; }
}
