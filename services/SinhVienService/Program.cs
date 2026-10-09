using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using SinhVienService.Data;
using SinhVienService.Middlewares;
using SinhVienService.Repositories;
using SinhVienService.Services;

// Cấu hình Serilog Logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Tích hợp Serilog
    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console()
        .ReadFrom.Configuration(ctx.Configuration));

    // Thêm Controller
    builder.Services.AddControllers();

    // Thêm Health Checks
    builder.Services.AddHealthChecks();

    // Thêm IHttpClientFactory để gọi kiểm tra ràng buộc sang DangKyService khi cần xóa
    builder.Services.AddHttpClient();

    // Cấu hình DbContext: Mặc định bật UseInMemoryDatabase để có thể chạy thử nghiệm ngay lập tức
    // Nếu muốn dùng SQL Server thực tế, chỉ cần sửa UseInMemoryDatabase = false trong appsettings.json
    bool useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", true);
    if (useInMemory)
    {
        builder.Services.AddDbContext<SinhVienDbContext>(options =>
            options.UseInMemoryDatabase("SinhVienDb"));
    }
    else
    {
        builder.Services.AddDbContext<SinhVienDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    }

    // Đăng ký Dependency Injection theo mô hình Repository & Service
    builder.Services.AddScoped<ISinhVienRepository, SinhVienRepository>();
    builder.Services.AddScoped<ISinhVienService, SinhVienAppService>();

    // Cấu hình Swagger / OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "SinhVienService API",
            Version = "v1",
            Description = "Dịch vụ quản lý Sinh viên trong hệ thống SOA Quản lý Đồ án Tốt nghiệp"
        });
    });

    // Cấu hình CORS để WebClient hoặc frontend có thể gọi được
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    var app = builder.Build();

    // Khởi tạo và nạp dữ liệu mẫu ban đầu nếu dùng InMemory Database
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<SinhVienDbContext>();
        db.Database.EnsureCreated();
    }

    // Middleware xử lý ngoại lệ tập trung (Global Exception Handling)
    app.UseMiddleware<GlobalExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SinhVienService API v1");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseCors("AllowAll");

    // Endpoint kiểm tra trạng thái hoạt động (Health check)
    app.MapHealthChecks("/health");

    app.MapControllers();

    Log.Information("Khởi động SinhVienService thành công tại cổng 5001...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "SinhVienService dừng hoạt động đột ngột vì ngoại lệ!");
}
finally
{
    Log.CloseAndFlush();
}
