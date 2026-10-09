namespace SinhVienService.Models;

/// <summary>
/// Đại diện cho bảng SINHVIEN trong CSDL của SinhVienService
/// </summary>
public class SinhVien
{
    /// <summary>
    /// Mã sinh viên (Khóa chính PK)
    /// </summary>
    public string MaSV { get; set; } = string.Empty;

    public string HoTen { get; set; } = string.Empty;

    public string Lop { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string SoDienThoai { get; set; } = string.Empty;
}
