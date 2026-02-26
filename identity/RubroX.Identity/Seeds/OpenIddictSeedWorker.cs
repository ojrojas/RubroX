using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using RubroX.Identity.Data;
using RubroX.Identity.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace RubroX.Identity.Seeds;

/// <summary>
/// Worker que siembra los datos iniciales de OpenIddict (aplicaciones, scopes) y ASP.NET Identity (roles, usuarios demo).
/// Se ejecuta una única vez al arrancar la aplicación.
/// </summary>
public sealed class OpenIddictSeedWorker(
    IServiceProvider serviceProvider,
    ILogger<OpenIddictSeedWorker> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        await SeedRolesAsync(scope, cancellationToken);
        await SeedOpenIddictApplicationsAsync(scope, cancellationToken);
        await SeedOpenIddictScopesAsync(scope, cancellationToken);
        await SeedDemoUsersAsync(scope, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    // =========================================================================
    // Roles
    // =========================================================================
    private async Task SeedRolesAsync(IServiceScope scope, CancellationToken ct)
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles =
        [
            "admin_presupuestal",
            "analista",
            "supervisor",
            "ordenador_gasto",
            "auditor"
        ];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation("Rol '{Role}' creado.", role);
            }
        }
    }

    // =========================================================================
    // OpenIddict — Aplicaciones cliente
    // =========================================================================
    private async Task SeedOpenIddictApplicationsAsync(IServiceScope scope, CancellationToken ct)
    {
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        // Cliente Angular SPA
        if (await manager.FindByClientIdAsync("rubrox-spa", ct) is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "rubrox-spa",
                ClientType = ClientTypes.Public,
                DisplayName = "RubroX SPA",
                RedirectUris =
                {
                    new Uri("http://localhost:4200/callback"),
                    new Uri("http://localhost:4200/silent-renew.html")
                },
                PostLogoutRedirectUris =
                {
                    new Uri("http://localhost:4200")
                },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                    "scp:offline_access",
                    "scp:rubrox.rubros",
                    "scp:rubrox.movimientos",
                    "scp:rubrox.flujos",
                    "scp:rubrox.reportes",
                    "scp:rubrox.auditoria",
                    "scp:rubrox.admin"
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange // PKCE obligatorio para SPA
                }
            }, ct);

            logger.LogInformation("Aplicación OpenIddict 'rubrox-spa' creada.");
        }
    }

    // =========================================================================
    // OpenIddict — Scopes de negocio
    // =========================================================================
    private async Task SeedOpenIddictScopesAsync(IServiceScope scope, CancellationToken ct)
    {
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        var scopes = new[]
        {
            ("rubrox.rubros", "Gestión de Rubros Presupuestales"),
            ("rubrox.movimientos", "Gestión de Movimientos (CDP, CRP, Pagos)"),
            ("rubrox.flujos", "Flujos de Aprobación"),
            ("rubrox.reportes", "Reportes de Ejecución"),
            ("rubrox.auditoria", "Auditoría del Sistema"),
            ("rubrox.admin", "Administración General")
        };

        foreach (var (name, displayName) in scopes)
        {
            if (await manager.FindByNameAsync(name, ct) is null)
            {
                await manager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    Name = name,
                    DisplayName = displayName,
                    Resources = { "rubrox-api" }
                }, ct);

                logger.LogInformation("Scope '{Scope}' creado.", name);
            }
        }
    }

    // =========================================================================
    // Usuarios demo
    // =========================================================================
    private async Task SeedDemoUsersAsync(IServiceScope scope, CancellationToken ct)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var usuarios = new[]
        {
            ("admin@rubrox.gov.co", "Admin@123!", "Administrador Presupuestal", "admin_presupuestal"),
            ("analista@rubrox.gov.co", "Analista@123!", "María García Analista", "analista"),
            ("supervisor@rubrox.gov.co", "Supervisor@123!", "Carlos Rodríguez Supervisor", "supervisor"),
            ("ordenador@rubrox.gov.co", "Ordenador@123!", "Jorge Martínez Ordenador", "ordenador_gasto"),
            ("auditor@rubrox.gov.co", "Auditor@123!", "Ana López Auditora", "auditor")
        };

        foreach (var (email, password, nombre, rol) in usuarios)
        {
            if (await userManager.FindByEmailAsync(email) is null)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    NombreCompleto = nombre,
                    EmailConfirmed = true,
                    Activo = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, rol);
                    logger.LogInformation("Usuario demo '{Email}' creado con rol '{Rol}'.", email, rol);
                }
                else
                {
                    logger.LogWarning("Error creando usuario '{Email}': {Errors}", email,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
