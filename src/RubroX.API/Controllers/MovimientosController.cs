using Microsoft.AspNetCore.Mvc;
using RubroX.Application.Abstractions;
using RubroX.Application.Commands.Movimientos.RegistrarCDP;
using RubroX.Domain.Aggregates.Movimientos;
using RubroX.Domain.Enums;

namespace RubroX.API.Controllers;

[ApiController]
[Route("api/v1/movimientos")]
[Produces("application/json")]
public sealed class MovimientosController(CommandDispatcher commands) : ControllerBase
{
    /// <summary>Registra un CDP (Certificado de Disponibilidad Presupuestal).</summary>
    [HttpPost("cdp")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegistrarCDP([FromBody] RegistrarCDPRequest request, CancellationToken ct = default)
    {
        var usuarioId = User.FindFirst("sub")?.Value ?? "system";
        var result = await commands.DispatchAsync<RegistrarCDPCommand, Guid>(
            new RegistrarCDPCommand(request.RubroId, request.Monto, request.Concepto, usuarioId, request.FechaVencimiento), ct);

        if (result.IsFailure) return BadRequest(new { error = result.Error });
        return CreatedAtAction(nameof(RegistrarCDP), new { id = result.Value }, new { id = result.Value });
    }
}

public sealed record RegistrarCDPRequest(
    Guid RubroId,
    decimal Monto,
    string Concepto,
    DateTimeOffset? FechaVencimiento = null);
