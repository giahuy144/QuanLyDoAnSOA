using DangKyService.Clients;
using DangKyService.Data;
using DangKyService.Middlewares;
using DangKyService.Repositories;
using DangKyService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Serilog;

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

    builder.Services.AddControllers();
    builder.Services.AddHealthChecks();

    // 1. Cấu hình DbContext riêng cho DangKyService
    bool useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", true);
    if (useInMemory)
    {
        builder.Services.AddDbContext<DangKyDbContext>(options =>
            options.UseInMemoryDatabase("DangKyDb"));
    }
    else
    {
        builder.Services.AddDbContext<DangKyDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    }

    // 2. Cấu hình Polly Policy: Retry 3 lần với Exponential Backoff (2s, 4s, 8s) khi gặp lỗi mạng/5xx
    var retryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                Log.Warning("Thử lại lần {RetryAttempt} sau {Seconds}s do lỗi: {StatusCode}",
                    retryAttempt, timespan.TotalSeconds, outcome.Result?.StatusCode);
            });

    // 3. Cấu hình Timeout Policy: Hủy request nếu quá 5 giây
    var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5));

    // 4. Đăng ký Typed Clients kết hợp Polly
    var sinhVienUrl = builder.Configuration["Services:SinhVienService"] ?? "http://localhost:5001";
    builder.Services.AddHttpClient<ISinhVienClient, SinhVienClient>(client =>
    {
        client.BaseAddress = new Uri(sinhVienUrl);
    })
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(timeoutPolicy);

    var deTaiUrl = builder.Configuration["Services:DeTaiService"] ?? "http://localhost:5002";
    builder.Services.AddHttpClient<IDeTaiClient, DeTaiClient>(client =>
    {
        client.BaseAddress = new Uri(deTaiUrl);
    })
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(timeoutPolicy);

    // 5. Đăng ký Repository & Service
    builder.Services.AddScoped<IDangKyRepository, DangKyRepository>();
    builder.Services.AddScoped<IDangKyService, DangKyAppService>();

    // 6. Cấu hình Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "DangKyService API",
            Version = "v1",
            Description = "Dịch vụ Quản lý Đăng ký Đề tài Tốt nghiệp (SOA Coordination Service)"
        });
    });

    // 7. Cấu hình CORS
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

    // Khởi tạo CSDL và seed data
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<DangKyDbContext>();
        db.Database.EnsureCreated();
    }

    // Middleware xử lý lỗi toàn cục
    app.UseMiddleware<GlobalExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "DangKyService API v1");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseCors("AllowAll");

    // Endpoint Health Check
    app.MapHealthChecks("/health");

    app.MapControllers();

    Log.Information("Khởi động DangKyService thành công tại cổng 5003...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "DangKyService dừng đột ngột!");
}
finally
{
    Log.CloseAndFlush();
}
