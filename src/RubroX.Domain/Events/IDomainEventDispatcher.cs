using RubroX.Domain.Events;

namespace RubroX.Domain.Events;

/// <summary>Dispatcher de domain events — implementado en Infrastructure.</summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
}

/// <summary>Handler para un tipo específico de domain event.</summary>
public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
