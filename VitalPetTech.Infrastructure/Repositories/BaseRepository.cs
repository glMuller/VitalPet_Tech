using Microsoft.EntityFrameworkCore;
using VitalPetTech.Domain.Interfaces;
using VitalPetTech.Infrastructure.Data;

namespace VitalPetTech.Infrastructure.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationContext Context;
    protected readonly DbSet<T> DbSet;

    protected BaseRepository(ApplicationContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    // Cada repositório concreto decide qual relacionamento carregar
    public abstract Task<T?> GetByIdAsync(int id);

    public abstract Task<bool> ExistsAsync(int id);

    public async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        // Entidade já rastreada: o EF detecta as mudanças sozinho
        if (Context.Entry(entity).State == EntityState.Detached)
            DbSet.Update(entity);

        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        DbSet.Remove(entity);
        await Context.SaveChangesAsync();
    }
}