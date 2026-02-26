using RubroX.Application.Abstractions;

namespace RubroX.Application.Commands.Rubros.CrearRubro;

public sealed record CrearRubroCommand(
    string Codigo,
    string Nombre,
    string Descripcion,
    int AnoFiscal,
    string Tipo,
    string Fuente,
    Guid? PadreId,
    string UsuarioId) : ICommand<Guid>;
