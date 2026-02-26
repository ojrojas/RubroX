using RubroX.Application.Abstractions;
using RubroX.Infrastructure.Persistence;

namespace RubroX.Infrastructure.Persistence;

public sealed class UnitOfWork(RubroXDbContext dbContext) : IUnitOfWork
{
    public Task<int> CommitAsync(CancellationToken ct = default) =>
        dbContext.SaveChangesAsync(ct);
}
