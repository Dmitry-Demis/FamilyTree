﻿using FamilyTree.DAL.Model;

namespace FamilyTree.DAL.Storage
{
    public interface IRepository<T> where T :
        class
        , IEntity
        , new()
    {
        Task<IEnumerable<T>?> GetAllAsync(CancellationToken cancel = default);
        Task<T?> GetAsync(int id, CancellationToken cancel = default);
        Task<T> AddAsync(T item, CancellationToken cancel = default);
        Task UpdateAsync(T item, CancellationToken cancel = default);
        Task RemoveAsync(int id, CancellationToken cancel = default);
        IQueryable<T>? Items { get; }

        Task ClearAllAsync(CancellationToken cancel = default);
    }
}
