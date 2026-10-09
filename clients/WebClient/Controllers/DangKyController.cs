using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Controllers;

/// <summary>
/// Đăng ký đồ án tốt nghiệp.
/// - Sinh viên: được phép xem và ĐĂNG KÝ đồ án (chỉ đăng ký cho chính mình).
/// - Giáo viên: có toàn quyền, bao gồm cả HỦY đăng ký của sinh viên.
/// => Đây chính là điểm thể hiện phân quyền theo vai trò trong hệ thống SOA.
/// </summary>
[Authorize]
public class DangKyController : Controller
{
    private readonly IApiClientService _apiService;

    public DangKyController(IApiClientService apiService)
    {
        _apiService = apiService;
    }

    /// <summary>True nếu người dùng hiện tại là Giáo viên.</summary>
    private bool IsGiaoVien => User.IsInRole("GiaoVien");

    /// <summary>Mã sinh viên được liên kết với tài khoản (claim ReferenceCode).</summary>
    private string? MaSVCuaToi => User.FindFirst("ReferenceCode")?.Value;

    public async Task<IActionResult> Index()
    {
        var list = await _apiService.GetDangKysAsync();

        // Sinh viên chỉ thấy các đăng ký của chính mình
        if (!IsGiaoVien && !string.IsNullOrEmpty(MaSVCuaToi))
        {
            list = list.Where(x => x.MaSV == MaSVCuaToi).ToList();
        }

        ViewBag.IsGiaoVien = IsGiaoVien;
        return View(list);
    }

    public async Task<IActionResult> Create()
    {
        var sinhViens = await _apiService.GetSinhViensAsync();
        var deTais = await _apiService.GetDeTaisAsync();

        var vm = new DangKyCreateViewModel
        {
            SinhViens = sinhViens,
            DeTais = deTais
        };

        // Sinh viên bị khóa ô chọn sinh viên và tự động gán mã của chính mình
        if (!IsGiaoVien)
        {
            vm.KhoaChonSinhVien = true;
            if (!string.IsNullOrEmpty(MaSVCuaToi))
            {
                vm.MaSV = MaSVCuaToi;
            }
        }

        ViewBag.IsGiaoVien = IsGiaoVien;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DangKyCreateViewModel model)
    {
        // Sinh viên bắt buộc phải dùng đúng mã của chính mình, không được đổi sang người khác
        if (!IsGiaoVien)
        {
            model.KhoaChonSinhVien = true;
            if (string.IsNullOrEmpty(MaSVCuaToi))
            {
                ModelState.AddModelError("", "Tài khoản của bạn chưa được liên kết với Mã sinh viên (ReferenceCode). Vui lòng liên hệ giáo viên.");
            }
            else
            {
                model.MaSV = MaSVCuaToi;   // Ghi đè mọi giá trị gửi lên từ form
            }
        }

        if (string.IsNullOrEmpty(model.MaSV) || string.IsNullOrEmpty(model.MaDT))
        {
            ModelState.AddModelError("", "Vui lòng chọn cả sinh viên và đề tài.");
            model.SinhViens = await _apiService.GetSinhViensAsync();
            model.DeTais = await _apiService.GetDeTaisAsync();
            ViewBag.IsGiaoVien = IsGiaoVien;
            return View(model);
        }

        var (success, error) = await _apiService.CreateDangKyAsync(model.MaSV, model.MaDT);
        if (success)
        {
            TempData["Success"] = "Đăng ký đề tài thành công!";
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = error ?? "Đăng ký thất bại.";
        model.SinhViens = await _apiService.GetSinhViensAsync();
        model.DeTais = await _apiService.GetDeTaisAsync();
        ViewBag.IsGiaoVien = IsGiaoVien;
        return View(model);
    }

    /// <summary>
    /// Hủy đăng ký: chỉ Giáo viên mới có quyền (sinh viên không được tự hủy).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "GiaoVien")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _apiService.DeleteDangKyAsync(id);
        if (success)
        {
            TempData["Success"] = "Đã hủy đăng ký thành công!";
        }
        else
        {
            TempData["Error"] = error ?? "Không thể hủy đăng ký.";
        }
        return RedirectToAction(nameof(Index));
    }
}
