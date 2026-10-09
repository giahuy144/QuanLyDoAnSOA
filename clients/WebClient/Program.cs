using WebClient.Services;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ MVC Controllers with Views
builder.Services.AddControllersWithViews();

// Đăng ký HttpClientFactory và ApiClientService để gọi đến 3 API services
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
