namespace DangKyService.Models;

/// <summary>
/// Đại diện cho bảng DANGKY trong CSDL riêng của DangKyService
/// Không dùng Foreign Key vật lý xuyên service (SOA Principle).
/// MaSV và MaDT được xác thực thông qua API call sang SinhVienService và DeTaiService.
/// </summary>
public class DangKy
{
    /// <summary>
    /// Mã đăng ký (Khóa chính PK)
    /// </summary>
    public int MaDK { get; set; }

    /// <summary>
    /// Mã sinh viên (Khóa ngoại logic liên dịch vụ)
    /// </summary>
    public string MaSV { get; set; } = string.Empty;

    /// <summary>
    /// Mã đề tài (Khóa ngoại logic liên dịch vụ)
    /// </summary>
    public string MaDT { get; set; } = string.Empty;

    /// <summary>
    /// Thời điểm đăng ký
    /// </summary>
    public DateTime NgayDangKy { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Trạng thái đăng ký: DaDangKy, ChoDuyet, DaHuy...
    /// </summary>
    public string TrangThai { get; set; } = "DaDangKy";
}
