using Microsoft.Extensions.DependencyInjection;
using RubroX.Domain.Common;

namespace RubroX.Application.Abstractions;

/// <summary>
/// Dispatcher de commands — resuelve el handler correspondiente via DI.
/// No usa MediatR: es una resolución directa por tipo.
/// </summary>
public sealed class CommandDispatcher(IServiceProvider serviceProvider)
{
    public async Task<Result> DispatchAsync<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        return await handler.HandleAsync(command, ct);
    }

    public async Task<Result<TResult>> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand<TResult>
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        return await handler.HandleAsync(command, ct);
    }
}

/// <summary>
/// Dispatcher de queries — resuelve el handler correspondiente via DI.
/// </summary>
public sealed class QueryDispatcher(IServiceProvider serviceProvider)
{
    public async Task<Result<TResult>> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken ct = default)
        where TQuery : IQuery<TResult>
    {
        var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
        return await handler.HandleAsync(query, ct);
    }
}
