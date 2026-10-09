using System.ComponentModel.DataAnnotations;

namespace DangKyService.DTOs;

/// <summary>
/// DTO chứa thông tin sinh viên nhận về khi gọi SinhVienService
/// </summary>
public class SinhVienDto
{
    public string MaSV { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string Lop { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
}

/// <summary>
/// DTO chứa thông tin đề tài nhận về khi gọi DeTaiService
/// </summary>
public class DeTaiDto
{
    public string MaDT { get; set; } = string.Empty;
    public string TenDT { get; set; } = string.Empty;
    public string MoTa { get; set; } = string.Empty;
    public string GiangVienHD { get; set; } = string.Empty;
    public int SoLuongToiDa { get; set; }
}

/// <summary>
/// DTO tiếp nhận yêu cầu đăng ký đề tài
/// </summary>
public class CreateDangKyDto
{
    [Required(ErrorMessage = "Mã sinh viên là bắt buộc")]
    public string MaSV { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mã đề tài là bắt buộc")]
    public string MaDT { get; set; } = string.Empty;
}

/// <summary>
/// DTO cập nhật trạng thái đăng ký
/// </summary>
public class UpdateDangKyDto
{
    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    public string TrangThai { get; set; } = string.Empty;
}

/// <summary>
/// DTO kết quả trả về, có thể kèm thông tin chi tiết sinh viên và đề tài được tổng hợp từ 2 service kia
/// </summary>
public class DangKyResponseDto
{
    public int MaDK { get; set; }
    public string MaSV { get; set; } = string.Empty;
    public string MaDT { get; set; } = string.Empty;
    public DateTime NgayDangKy { get; set; }
    public string TrangThai { get; set; } = string.Empty;

    // Thông tin làm giàu (Enriched data từ các service khác)
    public string? HoTenSinhVien { get; set; }
    public string? LopSinhVien { get; set; }
    public string? TenDeTai { get; set; }
    public string? GiangVienHD { get; set; }
}
