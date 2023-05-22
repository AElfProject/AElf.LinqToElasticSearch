using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Nest;
using Remotion.Linq.Clauses;
using Volo.Abp.Domain.Entities;

namespace LinqToElasticSearch.Provider
{
    public interface ILinqQueryRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>, new()
    {
        ILinqQueryRepository<TEntity,TKey> WhereClause(Expression<Func<TEntity, bool>> where);

        ILinqQueryRepository<TEntity,TKey> OrderASC(Expression<Func<TEntity, object>> keySelector);

        ILinqQueryRepository<TEntity,TKey> OrderDesc(Expression<Func<TEntity, object>> keySelector);

        ILinqQueryRepository<TEntity,TKey> Skip(int skip);

        ILinqQueryRepository<TEntity,TKey> Take(int take);

        long Count(Expression<Func<TEntity, bool>> where);

        List<TEntity> ToList();
    }
}