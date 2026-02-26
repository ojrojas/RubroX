using FluentAssertions;
using RubroX.Domain.Aggregates.Rubros;
using RubroX.Domain.Aggregates.Rubros.Events;
using RubroX.Domain.Enums;
using RubroX.Domain.ValueObjects;
using Xunit;

namespace RubroX.Domain.Tests.Aggregates;

public sealed class RubroTests
{
    private static Rubro CrearRubroValido(string codigo = "01.01", string nombre = "Servicios Personales") =>
        Rubro.Crear(
            CodigoPresupuestal.Parse(codigo),
            nombre,
            "Descripción de prueba",
            AnoFiscal.Parse(2025),
            TipoRubro.Funcionamiento,
            FuenteFinanciamiento.Nacion,
            null,
            "usuario-test").Value;

    [Fact]
    public void Crear_ConDatosValidos_CreaRubroActivo()
    {
        var rubro = CrearRubroValido();

        rubro.Should().NotBeNull();
        rubro.Estado.Should().Be(EstadoRubro.Activo);
        rubro.SaldoInicial.Valor.Should().Be(0m);
        rubro.SaldoComprometido.Valor.Should().Be(0m);
        rubro.SaldoEjecutado.Valor.Should().Be(0m);
    }

    [Fact]
    public void Crear_ConDatosValidos_EmiteRubroCreatedEvent()
    {
        var rubro = CrearRubroValido();

        rubro.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<RubroCreatedEvent>();
    }

    [Fact]
    public void Crear_ConNombreVacio_RetornaFailure()
    {
        var result = Rubro.Crear(
            CodigoPresupuestal.Parse("01.01"), string.Empty, "Descripción",
            AnoFiscal.Parse(2025), TipoRubro.Funcionamiento, FuenteFinanciamiento.Nacion, null, "test");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("nombre");
    }

    [Fact]
    public void SaldoDisponible_SinMovimientos_EsIgualAlInicial()
    {
        var rubro = CrearRubroValido();
        rubro.AsignarPresupuesto(Saldo.Parse(1_000_000m), "user");

        rubro.SaldoDisponible.Should().Be(1_000_000m);
    }

    [Fact]
    public void SaldoDisponible_ConSaldoComprometido_EsInicialMenosComprometido()
    {
        var rubro = CrearRubroValido();
        rubro.AsignarPresupuesto(Saldo.Parse(1_000_000m), "user");
        rubro.ReservarSaldo(Saldo.Parse(300_000m), "CDP-2025-0001");

        rubro.SaldoDisponible.Should().Be(700_000m);
        rubro.SaldoComprometido.Valor.Should().Be(300_000m);
    }

    [Fact]
    public void ReservarSaldo_ConMontoSuperiorAlDisponible_RetornaFailure()
    {
        var rubro = CrearRubroValido();
        rubro.AsignarPresupuesto(Saldo.Parse(100_000m), "user");

        var result = rubro.ReservarSaldo(Saldo.Parse(200_000m), "CDP-2025-0001");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Saldo insuficiente");
    }

    [Fact]
    public void EjecutarSaldo_MueveDeComprometidoAEjecutado()
    {
        var rubro = CrearRubroValido();
        rubro.AsignarPresupuesto(Saldo.Parse(1_000_000m), "user");
        rubro.ReservarSaldo(Saldo.Parse(500_000m), "CDP-2025-0001");

        var result = rubro.EjecutarSaldo(Saldo.Parse(300_000m), "CRP-2025-0001");

        result.IsSuccess.Should().BeTrue();
        rubro.SaldoComprometido.Valor.Should().Be(200_000m);
        rubro.SaldoEjecutado.Valor.Should().Be(300_000m);
        rubro.SaldoDisponible.Should().Be(500_000m);
    }

    [Fact]
    public void PorcentajeEjecucion_CalculaCorrectamente()
    {
        var rubro = CrearRubroValido();
        rubro.AsignarPresupuesto(Saldo.Parse(1_000_000m), "user");
        rubro.ReservarSaldo(Saldo.Parse(500_000m), "CDP-2025-0001");
        rubro.EjecutarSaldo(Saldo.Parse(500_000m), "CRP-2025-0001");

        rubro.PorcentajeEjecucion.Should().Be(50m);
    }

    [Fact]
    public void Cerrar_SinSaldoComprometido_CierraExitosamente()
    {
        var rubro = CrearRubroValido();
        rubro.AsignarPresupuesto(Saldo.Parse(100_000m), "user");

        var result = rubro.Cerrar("admin");

        result.IsSuccess.Should().BeTrue();
        rubro.Estado.Should().Be(EstadoRubro.Cerrado);
    }

    [Fact]
    public void Cerrar_ConSaldoComprometidoPendiente_RetornaFailure()
    {
        var rubro = CrearRubroValido();
        rubro.AsignarPresupuesto(Saldo.Parse(100_000m), "user");
        rubro.ReservarSaldo(Saldo.Parse(50_000m), "CDP-2025-0001");

        var result = rubro.Cerrar("admin");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("comprometido");
    }

    [Fact]
    public void AsignarPresupuesto_ARubroCerrado_RetornaFailure()
    {
        var rubro = CrearRubroValido();
        rubro.Cerrar("admin");

        var result = rubro.AsignarPresupuesto(Saldo.Parse(100_000m), "user");

        result.IsFailure.Should().BeTrue();
    }
}
