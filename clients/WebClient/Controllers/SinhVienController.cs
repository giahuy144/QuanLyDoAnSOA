using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Controllers;

/// <summary>
/// Quản lý hồ sơ Sinh viên: CHỈ GIÁO VIÊN mới được truy cập (CRUD toàn phần).
/// Sinh viên truy cập sẽ bị chuyển hướng sang /Account/AccessDenied (403).
/// </summary>
[Authorize(Roles = "GiaoVien")]
public class SinhVienController : Controller
{
    private readonly IApiClientService _apiService;

    public SinhVienController(IApiClientService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _apiService.GetSinhViensAsync();
        return View(list);
    }

    public IActionResult Create()
    {
        return View(new SinhVienViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SinhVienViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var (success, error) = await _apiService.CreateSinhVienAsync(model);
        if (success)
        {
            TempData["Success"] = "Thêm mới sinh viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", error ?? "Không thể thêm mới sinh viên.");
        return View(model);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var sv = await _apiService.GetSinhVienByIdAsync(id);
        if (sv == null) return NotFound();
        return View(sv);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, SinhVienViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var (success, error) = await _apiService.UpdateSinhVienAsync(id, model);
        if (success)
        {
            TempData["Success"] = "Cập nhật sinh viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", error ?? "Không thể cập nhật sinh viên.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var (success, error) = await _apiService.DeleteSinhVienAsync(id);
        if (success)
        {
            TempData["Success"] = $"Đã xóa sinh viên {id} thành công!";
        }
        else
        {
            TempData["Error"] = error ?? "Không thể xóa sinh viên.";
        }
        return RedirectToAction(nameof(Index));
    }
}
