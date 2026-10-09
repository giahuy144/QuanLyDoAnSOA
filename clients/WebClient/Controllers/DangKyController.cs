using Microsoft.AspNetCore.Mvc;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Controllers;

public class DangKyController : Controller
{
    private readonly IApiClientService _apiService;

    public DangKyController(IApiClientService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _apiService.GetDangKysAsync();
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
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DangKyCreateViewModel model)
    {
        if (string.IsNullOrEmpty(model.MaSV) || string.IsNullOrEmpty(model.MaDT))
        {
            ModelState.AddModelError("", "Vui lòng chọn cả sinh viên và đề tài.");
            model.SinhViens = await _apiService.GetSinhViensAsync();
            model.DeTais = await _apiService.GetDeTaisAsync();
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
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
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
