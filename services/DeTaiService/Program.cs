using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using DeTaiService.Data;
using DeTaiService.Middlewares;
using DeTaiService.Repositories;
using DeTaiService.Services;

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

    // Thêm Controllers
    builder.Services.AddControllers();

    // Thêm Health Checks
    builder.Services.AddHealthChecks();

    // Thêm IHttpClientFactory để giao tiếp SOA (kiểm tra DangKyService khi xóa đề tài)
    builder.Services.AddHttpClient();

    // Cấu hình DbContext riêng cho DeTaiService
    bool useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", true);
    if (useInMemory)
    {
        builder.Services.AddDbContext<DeTaiDbContext>(options =>
            options.UseInMemoryDatabase("DeTaiDb"));
    }
    else
    {
        builder.Services.AddDbContext<DeTaiDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    }

    // Đăng ký Dependency Injection
    builder.Services.AddScoped<IDeTaiRepository, DeTaiRepository>();
    builder.Services.AddScoped<IDeTaiService, DeTaiAppService>();

    // Cấu hình Swagger / OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "DeTaiService API",
            Version = "v1",
            Description = "Dịch vụ quản lý Đề tài tốt nghiệp trong hệ thống SOA"
        });
    });

    // Cấu hình CORS
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

    // Khởi tạo Database và nạp dữ liệu mẫu
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<DeTaiDbContext>();
        db.Database.EnsureCreated();
    }

    // Middleware bắt lỗi toàn cục
    app.UseMiddleware<GlobalExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "DeTaiService API v1");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseCors("AllowAll");

    // Endpoint Health Check
    app.MapHealthChecks("/health");

    app.MapControllers();

    Log.Information("Khởi động DeTaiService thành công tại cổng 5002...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "DeTaiService dừng hoạt động đột ngột vì ngoại lệ!");
}
finally
{
    Log.CloseAndFlush();
}
