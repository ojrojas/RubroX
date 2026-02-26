using Microsoft.AspNetCore.Authentication.JwtBearer;
using RubroX.API.Middleware;
using RubroX.Application.Extensions;
using RubroX.Infrastructure.Extensions;
using Serilog;

// =========================================================================
// Bootstrap Serilog antes de crear el host
// =========================================================================
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // =========================================================================
    // Logging
    // =========================================================================
    builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    // =========================================================================
    // Servicios Application + Infrastructure
    // =========================================================================
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // =========================================================================
    // Auth — JWT Bearer con OpenIddict como authority
    // =========================================================================
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["Auth:Authority"];
            options.Audience = builder.Configuration["Auth:Audience"];
            options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminPresupuestal", p => p.RequireRole("admin_presupuestal"));
        options.AddPolicy("Analista", p => p.RequireRole("analista", "admin_presupuestal"));
        options.AddPolicy("Supervisor", p => p.RequireRole("supervisor", "admin_presupuestal"));
        options.AddPolicy("OrdenadorGasto", p => p.RequireRole("ordenador_gasto", "admin_presupuestal"));
        options.AddPolicy("Auditor", p => p.RequireRole("auditor", "admin_presupuestal"));
    });

    // =========================================================================
    // API
    // =========================================================================
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    builder.Services.AddCors(options =>
        options.AddPolicy("AllowFrontend", policy =>
            policy.WithOrigins(builder.Configuration["Cors:AllowedOrigins"]?.Split(',') ?? ["http://localhost:4200"])
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()));

    // =========================================================================
    // Build
    // =========================================================================
    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowFrontend");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTimeOffset.UtcNow }))
       .WithTags("Health");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "RubroX API terminó inesperadamente.");
}
finally
{
    Log.CloseAndFlush();
}
