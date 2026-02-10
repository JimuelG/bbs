using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<T?> GetEntityWithSpec(ISpecification<T> spec);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    IQueryable<TResult> GetQueryableWithProjection<TResult>(ISpecification<T> spec, Expression<Func<T, TResult>> selectExpression);
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    bool Exists(int id);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task<int> CountAsync(ISpecification<T> spec);
}

