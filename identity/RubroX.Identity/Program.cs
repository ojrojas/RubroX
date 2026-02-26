using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RubroX.Identity.Data;
using RubroX.Identity.Models;
using RubroX.Identity.Seeds;
using Serilog;
using static OpenIddict.Abstractions.OpenIddictConstants;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    // =========================================================================
    // Database
    // =========================================================================
    builder.Services.AddDbContext<IdentityDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityDb"));
        options.UseOpenIddict();
    });

    // =========================================================================
    // ASP.NET Identity
    // =========================================================================
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 12;
        options.Password.RequireUppercase = true;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = true;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false; // dev mode
    })
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();

    // =========================================================================
    // OpenIddict
    // =========================================================================
    builder.Services.AddOpenIddict()
        .AddCore(options =>
        {
            options.UseEntityFrameworkCore()
                   .UseDbContext<IdentityDbContext>();
        })
        .AddServer(options =>
        {
            // Endpoints
            options.SetAuthorizationEndpointUris("/connect/authorize")
                   .SetTokenEndpointUris("/connect/token")
                   .SetEndSessionEndpointUris("/connect/logout")
                   .SetUserInfoEndpointUris("/connect/userinfo")
                   .SetIntrospectionEndpointUris("/connect/introspect");

            // Flows habilitados
            options.AllowAuthorizationCodeFlow()
                   .AllowRefreshTokenFlow();

            // PKCE obligatorio
            options.RequireProofKeyForCodeExchange();

            // Scopes
            options.RegisterScopes(
                Scopes.Email, Scopes.Profile, Scopes.Roles, Scopes.OfflineAccess,
                "rubrox.rubros", "rubrox.movimientos", "rubrox.flujos",
                "rubrox.reportes", "rubrox.auditoria", "rubrox.admin");

            // Tokens
            options.SetAccessTokenLifetime(TimeSpan.FromMinutes(15));
            options.SetRefreshTokenLifetime(TimeSpan.FromHours(8));

            if (builder.Environment.IsDevelopment())
            {
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();
                options.UseAspNetCore()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableTokenEndpointPassthrough()
                       .EnableEndSessionEndpointPassthrough()
                       .EnableUserInfoEndpointPassthrough()
                       .DisableTransportSecurityRequirement(); // solo dev
            }
        })
        .AddValidation(options =>
        {
            options.UseLocalServer();
            options.UseAspNetCore();
        });

    // =========================================================================
    // MVC + Razor Pages para UI de login/consent
    // =========================================================================
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

    // =========================================================================
    // Seed worker
    // =========================================================================
    builder.Services.AddHostedService<OpenIddictSeedWorker>();

    builder.Services.AddCors(options =>
        options.AddPolicy("AllowAll", p => p
            .AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

    // =========================================================================
    // Build
    // =========================================================================
    var app = builder.Build();

    // Auto-apply migrations en desarrollo
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    app.UseSerilogRequestLogging();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapRazorPages();
    app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Identity Server terminó inesperadamente.");
}
finally
{
    Log.CloseAndFlush();
}
