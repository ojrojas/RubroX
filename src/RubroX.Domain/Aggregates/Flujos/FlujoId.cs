namespace RubroX.Domain.Aggregates.Flujos;

public readonly record struct FlujoId(Guid Value)
{
    public static FlujoId New() => new(Guid.NewGuid());
    public static FlujoId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}

public readonly record struct PasoId(Guid Value)
{
    public static PasoId New() => new(Guid.NewGuid());
    public static PasoId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
