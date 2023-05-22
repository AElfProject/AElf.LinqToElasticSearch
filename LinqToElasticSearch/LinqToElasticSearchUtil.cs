using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Nest;
using NUnit.Framework;

namespace LinqToElasticSearch
{
    public class LinqToElasticSearchUtil
    {
        
        public static List<TSource> Query<TSource,TKey>(ElasticClient client, string index, Expression<Func<TSource, bool>> where, List<Tuple<Expression<Func<TSource, object>>,string>> sortKeys, int from, int size)
        {
            var queryable = new ElasticQueryable<TSource>(client,index);
            queryable = (ElasticQueryable<TSource>)queryable.Where(where.ToString());
            if (sortKeys != null)
            {
                foreach (var sortKey in sortKeys)
                {
                    Expression<Func<TSource, Object>> keySelector = sortKey.Item1;
                    string sortType = sortKey.Item2;
                    string key = ExpressionHelper.GetMemberNameUnaryExpression(keySelector);
                    queryable =  (ElasticQueryable<TSource>)queryable.OrderBy(key + " " + sortType);
                }
            }
            if (from > 0)
            {
                queryable = (ElasticQueryable<TSource>)queryable.Skip(from);
            }

            if (size > 0)
            {
                queryable = (ElasticQueryable<TSource>)queryable.Take(size);
            }

            var results =  queryable.ToList();
            return results;
        }
        
        public static int Count<TSource,TKey>(ElasticClient client, string index, Expression<Func<TSource, bool>> where)
        {
            var queryable = new ElasticQueryable<TSource>(client,index);
            int count =  queryable.Count(where.ToString());
            return count;
        }
    }
}