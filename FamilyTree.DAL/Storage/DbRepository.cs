using FamilyTree.DAL.Model;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.DAL.Storage;

internal class DbRepository<T>(FamilyTreeDbContext db) : IRepository<T> where T :
    class
    , IEntity
    , new()
{
    private readonly DbSet<T> _set = db.Set<T>();
    private static bool AutoSaveChanges => true;

    public async Task<T?> GetAsync(int id, CancellationToken cancel = default) => await _set
        .SingleOrDefaultAsync(item => item.Id == id, cancel)
        .ConfigureAwait(false);

    public async Task<T> AddAsync(T item, CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        db.Entry(item).State = EntityState.Added;
        if (AutoSaveChanges)
            await db.SaveChangesAsync(cancel).ConfigureAwait(false);
        return item;
    }

    public async Task UpdateAsync(T item, CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        db.Entry(item).State = EntityState.Modified;
        if (AutoSaveChanges)
            await db.SaveChangesAsync(cancel).ConfigureAwait(false);
    }

    public async Task RemoveAsync(int id, CancellationToken cancel = default)
    {
        db.Remove(new T { Id = id });
        if (AutoSaveChanges)
            await db.SaveChangesAsync(cancel).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>?> GetAllAsync(CancellationToken cancel = default) => await _set.ToListAsync(cancel).ConfigureAwait(false);

    public IQueryable<T>? Items => _set;

    /// <summary>
    /// Удаление всех записей из таблицы T.
    /// </summary>
    public async Task ClearAllAsync(CancellationToken cancel = default)
    {
        try
        {
            Console.WriteLine($"Удаление всех записей из таблицы {typeof(T).Name}...");
            _set.RemoveRange(_set);
            if (AutoSaveChanges)
                await db.SaveChangesAsync(cancel).ConfigureAwait(false);
            Console.WriteLine($"Все записи из таблицы {typeof(T).Name} успешно удалены.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении всех данных: {ex.Message}");
            throw;
        }
    }
}
