namespace RubroX.Domain.Aggregates.Rubros;

public readonly record struct RubroId(Guid Value)
{
    public static RubroId New() => new(Guid.NewGuid());
    public static RubroId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
