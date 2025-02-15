﻿using System.Linq.Expressions;

namespace DataAccess.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string? includeProperties = null);
        IEnumerable<T> GetRange(Expression<Func<T, bool>> filter, string? includeProperties = null);
        T? Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
