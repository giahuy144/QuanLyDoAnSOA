using Microsoft.AspNetCore.Mvc;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Controllers;

public class DeTaiController : Controller
{
    private readonly IApiClientService _apiService;

    public DeTaiController(IApiClientService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _apiService.GetDeTaisAsync();
        return View(list);
    }

    public IActionResult Create()
    {
        return View(new DeTaiViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DeTaiViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var (success, error) = await _apiService.CreateDeTaiAsync(model);
        if (success)
        {
            TempData["Success"] = "Thêm mới đề tài thành công!";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", error ?? "Không thể thêm mới đề tài.");
        return View(model);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var dt = await _apiService.GetDeTaiByIdAsync(id);
        if (dt == null) return NotFound();
        return View(dt);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, DeTaiViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var (success, error) = await _apiService.UpdateDeTaiAsync(id, model);
        if (success)
        {
            TempData["Success"] = "Cập nhật đề tài thành công!";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", error ?? "Không thể cập nhật đề tài.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var (success, error) = await _apiService.DeleteDeTaiAsync(id);
        if (success)
        {
            TempData["Success"] = $"Đã xóa đề tài {id} thành công!";
        }
        else
        {
            TempData["Error"] = error ?? "Không thể xóa đề tài.";
        }
        return RedirectToAction(nameof(Index));
    }
}
