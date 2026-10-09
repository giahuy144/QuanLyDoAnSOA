using AuthService.Data;
using AuthService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

// Cấu hình Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console()
        .ReadFrom.Configuration(ctx.Configuration));

    builder.Services.AddControllers();
    builder.Services.AddHealthChecks();

    // Cấu hình DbContext riêng cho AuthService
    bool useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", true);
    if (useInMemory)
    {
        builder.Services.AddDbContext<AuthDbContext>(options =>
            options.UseInMemoryDatabase("AuthDb"));
    }
    else
    {
        builder.Services.AddDbContext<AuthDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    }

    // Đăng ký Service
    builder.Services.AddScoped<IAuthService, AuthServiceImplementation>();

    // Cấu hình Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "AuthService API",
            Version = "v1",
            Description = "Dịch vụ Xác thực và Phân quyền (Authentication & Authorization) trong hệ thống SOA"
        });
    });

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

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        db.Database.EnsureCreated();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthService API v1");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseCors("AllowAll");

    app.MapHealthChecks("/health");
    app.MapControllers();

    Log.Information("Khởi động AuthService thành công tại cổng 5004...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "AuthService dừng đột ngột!");
}
finally
{
    Log.CloseAndFlush();
}
