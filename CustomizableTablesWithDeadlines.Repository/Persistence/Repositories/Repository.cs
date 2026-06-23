using System.Linq.Expressions;
using CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;
using CustomizableTablesWithDeadlines.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace CustomizableTablesWithDeadlines.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual void Update(T entity) => DbSet.Update(entity);

    public virtual void Delete(T entity) => DbSet.Remove(entity);

    public virtual Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().ToListAsync(cancellationToken);

    public virtual async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
}
