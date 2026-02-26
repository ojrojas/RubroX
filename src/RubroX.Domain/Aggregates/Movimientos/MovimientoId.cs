namespace RubroX.Domain.Aggregates.Movimientos;

public readonly record struct MovimientoId(Guid Value)
{
    public static MovimientoId New() => new(Guid.NewGuid());
    public static MovimientoId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
