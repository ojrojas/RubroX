using FluentAssertions;
using RubroX.Domain.ValueObjects;
using Xunit;

namespace RubroX.Domain.Tests.ValueObjects;

public sealed class CodigoPresupuestalTests
{
    [Theory]
    [InlineData("01")]
    [InlineData("01.01")]
    [InlineData("01.01.1000")]
    [InlineData("01.01.1000.01")]
    public void Create_ConFormatoValido_RetornaSuccess(string codigo)
    {
        var result = CodigoPresupuestal.Create(codigo);
        result.IsSuccess.Should().BeTrue();
        result.Value.Valor.Should().Be(codigo);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("1")]         // un solo dígito
    [InlineData("ABC")]
    [InlineData("01-01")]     // separador incorrecto
    [InlineData("01.01.1000.01.99")]  // demasiados niveles
    public void Create_ConFormatoInvalido_RetornaFailure(string codigo)
    {
        var result = CodigoPresupuestal.Create(codigo);
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Nivel_CodigoRaiz_EsUno()
    {
        var codigo = CodigoPresupuestal.Parse("01");
        codigo.Nivel.Should().Be(1);
    }

    [Fact]
    public void Nivel_CodigoCuartoNivel_EsCuatro()
    {
        var codigo = CodigoPresupuestal.Parse("01.01.1000.01");
        codigo.Nivel.Should().Be(4);
    }

    [Fact]
    public void Padre_DeCodigoHijo_RetornaCodigoPadre()
    {
        var hijo = CodigoPresupuestal.Parse("01.01.1000.01");
        hijo.Padre!.Valor.Should().Be("01.01.1000");
    }

    [Fact]
    public void Padre_DeCodigoRaiz_RetornaNull()
    {
        var raiz = CodigoPresupuestal.Parse("01");
        raiz.Padre.Should().BeNull();
    }

    [Fact]
    public void Igualdad_DosCodigosIguales_SonIguales()
    {
        var a = CodigoPresupuestal.Parse("01.01");
        var b = CodigoPresupuestal.Parse("01.01");
        a.Should().Be(b);
    }
}

public sealed class SaldoTests
{
    [Fact]
    public void Create_ConValorPositivo_RetornaSuccess()
    {
        var result = Saldo.Create(100_000m);
        result.IsSuccess.Should().BeTrue();
        result.Value.Valor.Should().Be(100_000m);
    }

    [Fact]
    public void Create_ConCero_RetornaSuccess()
    {
        var result = Saldo.Create(0m);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_ConValorNegativo_RetornaFailure()
    {
        var result = Saldo.Create(-1m);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("negativo");
    }

    [Fact]
    public void Suma_DosSaldos_RetornaSumaCombinada()
    {
        var a = Saldo.Parse(300_000m);
        var b = Saldo.Parse(200_000m);
        var resultado = a + b;
        resultado.Valor.Should().Be(500_000m);
    }

    [Fact]
    public void Resta_MayorMenosMenor_RetornaSuccess()
    {
        var a = Saldo.Parse(500_000m);
        var b = Saldo.Parse(200_000m);
        var resultado = a - b;
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Valor.Should().Be(300_000m);
    }

    [Fact]
    public void Resta_MenorMayorMenor_RetornaFailure()
    {
        var a = Saldo.Parse(100_000m);
        var b = Saldo.Parse(200_000m);
        var resultado = a - b;
        resultado.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void EsSuficientePara_ConSaldoMayor_RetornaTrue()
    {
        var disponible = Saldo.Parse(500_000m);
        var requerido = Saldo.Parse(200_000m);
        disponible.EsSuficientePara(requerido).Should().BeTrue();
    }
}

public sealed class AnoFiscalTests
{
    [Theory]
    [InlineData(2000)]
    [InlineData(2025)]
    [InlineData(2099)]
    public void Create_ConAnoValido_RetornaSuccess(int ano)
    {
        var result = AnoFiscal.Create(ano);
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(1999)]
    [InlineData(2100)]
    [InlineData(0)]
    public void Create_ConAnoFueraDeRango_RetornaFailure(int ano)
    {
        var result = AnoFiscal.Create(ano);
        result.IsFailure.Should().BeTrue();
    }
}
