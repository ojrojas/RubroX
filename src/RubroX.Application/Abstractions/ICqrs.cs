using RubroX.Domain.Common;

namespace RubroX.Application.Abstractions;

/// <summary>Marca un comando del sistema. Sin valor de retorno.</summary>
public interface ICommand;

/// <summary>Marca un comando que retorna un resultado tipado.</summary>
public interface ICommand<TResult>;

/// <summary>Handler para ICommand (sin retorno).</summary>
public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command, CancellationToken ct = default);
}

/// <summary>Handler para ICommand con retorno tipado.</summary>
public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken ct = default);
}

/// <summary>Marca una query del sistema.</summary>
public interface IQuery<TResult>;

/// <summary>Handler para IQuery.</summary>
public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken ct = default);
}

/// <summary>Unidad de trabajo para persistencia atómica.</summary>
public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
