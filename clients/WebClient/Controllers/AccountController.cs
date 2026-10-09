using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Controllers;

/// <summary>
/// Controller xử lý Đăng ký / Đăng nhập / Đăng xuất.
/// WebClient KHÔNG tự kiểm tra mật khẩu: nó ủy quyền hoàn toàn cho AuthService (cổng 5004).
/// Khi AuthService trả về JWT, WebClient mới tạo phiên đăng nhập (Cookie) cho người dùng.
/// </summary>
public class AccountController : Controller
{
    private readonly IApiClientService _apiService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IApiClientService apiService, ILogger<AccountController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    #region Đăng nhập

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // Nếu đã đăng nhập rồi thì không cho vào trang Login nữa
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid) return View(model);

        // Gọi AuthService để xác thực
        var (success, error, data) = await _apiService.LoginAsync(model);

        if (!success || data == null)
        {
            ModelState.AddModelError("", error ?? "Đăng nhập thất bại.");
            return View(model);
        }

        // Tạo danh sách Claim từ kết quả AuthService trả về
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, data.Username),
            new Claim(ClaimTypes.Name, data.Username),
            new Claim(ClaimTypes.Role, data.Role),              // "GiaoVien" | "SinhVien"
            new Claim("FullName", data.FullName),
            new Claim("JwtToken", data.Token)                   // Giữ JWT để truyền sang các Service khác
        };

        // ReferenceCode = Mã Giảng viên / Mã Sinh viên được liên kết
        if (!string.IsNullOrWhiteSpace(data.ReferenceCode))
        {
            claims.Add(new Claim("ReferenceCode", data.ReferenceCode));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        // Lưu JWT vào Session để ApiClientService dùng khi gọi API
        HttpContext.Session.SetString("JwtToken", data.Token);
        HttpContext.Session.SetString("UserRole", data.Role);
        HttpContext.Session.SetString("FullName", data.FullName);

        _logger.LogInformation("Người dùng [{Username}] đăng nhập với vai trò {Role}", data.Username, data.Role);

        TempData["Success"] = $"Đăng nhập thành công! Xin chào {data.FullName} ({(data.Role == "GiaoVien" ? "Giảng viên" : "Sinh viên")}).";
        return RedirectToLocal(returnUrl, data.Role);
    }

    #endregion

    #region Đăng ký

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel { Role = "SinhVien" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        // Chỉ chấp nhận 2 vai trò hợp lệ, tránh người dùng giả mạo role
        if (model.Role != "GiaoVien" && model.Role != "SinhVien")
        {
            ModelState.AddModelError(nameof(model.Role), "Vai trò không hợp lệ.");
        }

        if (!ModelState.IsValid) return View(model);

        var (success, error, data) = await _apiService.RegisterAsync(model);

        if (!success || data == null)
        {
            ModelState.AddModelError("", error ?? "Đăng ký thất bại.");
            return View(model);
        }

        // Đăng ký xong thì đăng nhập luôn cho tiện demo
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, data.Username),
            new Claim(ClaimTypes.Name, data.Username),
            new Claim(ClaimTypes.Role, data.Role),
            new Claim("FullName", data.FullName),
            new Claim("JwtToken", data.Token)
        };

        if (!string.IsNullOrWhiteSpace(data.ReferenceCode))
        {
            claims.Add(new Claim("ReferenceCode", data.ReferenceCode));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = false, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) });

        HttpContext.Session.SetString("JwtToken", data.Token);
        HttpContext.Session.SetString("UserRole", data.Role);
        HttpContext.Session.SetString("FullName", data.FullName);

        TempData["Success"] = "Đăng ký tài khoản thành công! Bạn đã được đăng nhập tự động.";
        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region Đăng xuất & Từ chối truy cập

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Success"] = "Bạn đã đăng xuất.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    #endregion

    private IActionResult RedirectToLocal(string? returnUrl, string? role = null)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        // Điều hướng theo vai trò sau khi đăng nhập:
        //  - Giáo viên: về Trang chủ (có đầy đủ chức năng)
        //  - Sinh viên: thẳng vào trang Đăng ký đồ án
        var currentRole = role ?? User.FindFirst(ClaimTypes.Role)?.Value;
        if (currentRole == "GiaoVien")
        {
            return RedirectToAction("Index", "Home");
        }
        return RedirectToAction("Index", "DangKy");
    }
}
