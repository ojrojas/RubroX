using Microsoft.AspNetCore.Mvc;
using RubroX.Application.Abstractions;
using RubroX.Application.Commands.Flujos.AprobarPaso;
using RubroX.Application.DTOs;
using RubroX.Application.Queries.Flujos;

namespace RubroX.API.Controllers;

[ApiController]
[Route("api/v1/flujos")]
[Produces("application/json")]
public sealed class FlujosController(CommandDispatcher commands, QueryDispatcher queries) : ControllerBase
{
    /// <summary>Lista los flujos pendientes de aprobación para el rol del usuario.</summary>
    [HttpGet("bandeja")]
    [ProducesResponseType<IReadOnlyList<FlujoDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Bandeja(CancellationToken ct = default)
    {
        var rol = User.FindFirst("role")?.Value ?? string.Empty;
        var result = await queries.DispatchAsync<ListarFlujosPendientesPorRolQuery, IReadOnlyList<FlujoDto>>(
            new ListarFlujosPendientesPorRolQuery(rol), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>Aprueba el paso actual de un flujo.</summary>
    [HttpPost("{id:guid}/aprobar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Aprobar(Guid id, [FromBody] AccionFlujoRequest request, CancellationToken ct = default)
    {
        var aprobadorId = User.FindFirst("sub")?.Value ?? "system";
        var rol = User.FindFirst("role")?.Value ?? string.Empty;

        var result = await commands.DispatchAsync(
            new AprobarPasoCommand(id, aprobadorId, rol, request.Comentario), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

public sealed record AccionFlujoRequest(string? Comentario = null, string? Motivo = null);
