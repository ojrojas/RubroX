using Microsoft.AspNetCore.Mvc;
using RubroX.Application.Abstractions;
using RubroX.Application.Commands.Rubros.AsignarPresupuesto;
using RubroX.Application.Commands.Rubros.CrearRubro;
using RubroX.Application.DTOs;
using RubroX.Application.Queries.Rubros;

namespace RubroX.API.Controllers;

[ApiController]
[Route("api/v1/rubros")]
[Produces("application/json")]
public sealed class RubrosController(CommandDispatcher commands, QueryDispatcher queries) : ControllerBase
{
    /// <summary>Lista todos los rubros de una vigencia fiscal.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<RubroResumenDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] int anoFiscal = 0, [FromQuery] string? estado = null,
        CancellationToken ct = default)
    {
        if (anoFiscal == 0) anoFiscal = DateTime.UtcNow.Year;
        var result = await queries.DispatchAsync<ListarRubrosQuery, IReadOnlyList<RubroResumenDto>>(
            new ListarRubrosQuery(anoFiscal, estado), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>Obtiene el detalle de un rubro por ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<RubroDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken ct = default)
    {
        var result = await queries.DispatchAsync<ObtenerRubroPorIdQuery, RubroDto>(
            new ObtenerRubroPorIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    /// <summary>Crea un nuevo rubro presupuestal.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Crear([FromBody] CrearRubroRequest request, CancellationToken ct = default)
    {
        var usuarioId = User.FindFirst("sub")?.Value ?? "system";
        var command = new CrearRubroCommand(
            request.Codigo, request.Nombre, request.Descripcion ?? string.Empty,
            request.AnoFiscal, request.Tipo, request.Fuente, request.PadreId, usuarioId);

        var result = await commands.DispatchAsync<CrearRubroCommand, Guid>(command, ct);
        if (result.IsFailure) return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(ObtenerPorId), new { id = result.Value }, new { id = result.Value });
    }

    /// <summary>Asigna la apropiación presupuestal inicial al rubro.</summary>
    [HttpPut("{id:guid}/presupuesto")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AsignarPresupuesto(Guid id,
        [FromBody] AsignarPresupuestoRequest request, CancellationToken ct = default)
    {
        var usuarioId = User.FindFirst("sub")?.Value ?? "system";
        var result = await commands.DispatchAsync(
            new AsignarPresupuestoCommand(id, request.Monto, usuarioId), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

// ===== Request Models =====

public sealed record CrearRubroRequest(
    string Codigo,
    string Nombre,
    string? Descripcion,
    int AnoFiscal,
    string Tipo,
    string Fuente,
    Guid? PadreId = null);

public sealed record AsignarPresupuestoRequest(decimal Monto);
