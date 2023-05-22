using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Nest;
using Volo.Abp.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using LinqToElasticSearch.Provider;
using Nest;
using NUnit.Framework;
using Volo.Abp.DependencyInjection;

namespace LinqToElasticSearch.Provider
{
    public class LinqQueryRepository<TEntity, TKey> : LinqQueryBaseRepository<TEntity, TKey>,
        ILinqQueryRepository<TEntity, TKey> ,ITransientDependency
        where TEntity : class, IEntity<TKey>, new()
    {
        private ElasticQueryable<TEntity> queryable;
        public LinqQueryRepository(IEsClientProvider esClientProvider, string index = null) : base(esClientProvider, index)
        {
            queryable = new ElasticQueryable<TEntity>(esClientProvider.GetClient(),IndexName);
        }

        public ILinqQueryRepository<TEntity, TKey> WhereClause(Expression<Func<TEntity, bool>> where)
        {
            if (queryable == null)
            {
                throw new Exception("queryable is null");
            }
            queryable = (ElasticQueryable<TEntity>)queryable.Where(where.ToString());
            return this;
        }

        public ILinqQueryRepository<TEntity, TKey> OrderASC(Expression<Func<TEntity, object>> keySelector)
        {
            if (queryable == null)
            {
                throw new Exception("queryable is null");
            }
            string key = ExpressionHelper.GetMemberNameUnaryExpression(keySelector);

            queryable = (ElasticQueryable<TEntity>)queryable.OrderBy(key + " " + "ASC");
            return this;
        }

        public ILinqQueryRepository<TEntity, TKey> OrderDesc(Expression<Func<TEntity, object>> keySelector)
        {
            if (queryable == null)
            {
                throw new Exception("queryable is null");
            }
            string key = ExpressionHelper.GetMemberNameUnaryExpression(keySelector);
            queryable = (ElasticQueryable<TEntity>)queryable.OrderBy(key + " " + "DESC");
            return this;
        }

        public ILinqQueryRepository<TEntity, TKey> Skip(int skip)
        {
            if (skip > 0)
            {
                queryable = (ElasticQueryable<TEntity>)queryable.Skip(skip);
            }

            return this;
        }

        public ILinqQueryRepository<TEntity, TKey> Take(int take)
        {
            if (take > 0)
            {
                queryable = (ElasticQueryable<TEntity>)queryable.Take(take);
            }

            return this;
        }

        public long Count(Expression<Func<TEntity, bool>> where)
        {
            if (queryable == null)
            {
                throw new Exception("queryable is null");
            }
            return queryable.Count(where.ToString());
        }

        public List<TEntity> ToList()
        {
            return queryable.ToList();
        }
    }
}
