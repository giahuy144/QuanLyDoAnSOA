using Microsoft.AspNetCore.Authentication.Cookies;
using WebClient.Services;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ MVC Controllers with Views
builder.Services.AddControllersWithViews();

// Cần để ApiClientService đọc được JWT của người dùng đang đăng nhập (HttpContext.User)
builder.Services.AddHttpContextAccessor();

// Session dùng để lưu JWT trong phiên làm việc
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Xác thực bằng Cookie: WebClient giữ phiên đăng nhập, JWT do AuthService (cổng 5004) cấp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Phân quyền theo vai trò: "GiaoVien" (toàn quyền) và "SinhVien" (chỉ đăng ký đồ án)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("GiaoVienOnly", policy => policy.RequireRole("GiaoVien"));
    options.AddPolicy("SinhVienOnly", policy => policy.RequireRole("SinhVien"));
});

// Đăng ký HttpClientFactory và ApiClientService để gọi đến các API services
builder.Services.AddHttpClient<IApiClientService, ApiClientService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();          // Phải đặt trước UseAuthentication
app.UseAuthentication();   // Xác định "bạn là ai"
app.UseAuthorization();    // Xác định "bạn được làm gì"

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
