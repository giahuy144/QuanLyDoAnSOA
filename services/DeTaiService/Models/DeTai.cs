namespace DeTaiService.Models;

/// <summary>
/// Đại diện cho bảng DETAI trong CSDL độc lập của DeTaiService
/// </summary>
public class DeTai
{
    /// <summary>
    /// Mã đề tài (Khóa chính PK)
    /// </summary>
    public string MaDT { get; set; } = string.Empty;

    public string TenDT { get; set; } = string.Empty;

    public string MoTa { get; set; } = string.Empty;

    public string GiangVienHD { get; set; } = string.Empty;

    /// <summary>
    /// Số lượng sinh viên tối đa có thể đăng ký đề tài này
    /// </summary>
    public int SoLuongToiDa { get; set; }
}
