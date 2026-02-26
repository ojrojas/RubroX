using Microsoft.AspNetCore.Identity;

namespace RubroX.Identity.Models;

/// <summary>Usuario extendido de la aplicación con campos institucionales.</summary>
public sealed class ApplicationUser : IdentityUser
{
    public string? NombreCompleto { get; set; }
    public string? EntidadId { get; set; }
    public string? DependenciaId { get; set; }
    public string? Cargo { get; set; }
    public bool Activo { get; set; } = true;
    public DateTimeOffset CreadoEn { get; set; } = DateTimeOffset.UtcNow;
}
