namespace RubroX.Domain.Events;

/// <summary>Marca un evento de dominio. Inmutable por contrato.</summary>
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredAt { get; }
}

/// <summary>Base para todos los domain events con metadatos comunes.</summary>
public abstract record DomainEvent(Guid EventId, DateTimeOffset OccurredAt) : IDomainEvent
{
    protected DomainEvent() : this(Guid.NewGuid(), DateTimeOffset.UtcNow) { }
}
