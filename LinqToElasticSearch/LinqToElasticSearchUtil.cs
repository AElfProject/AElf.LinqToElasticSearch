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
            //排序
            foreach (var sortKey in sortKeys)
            {
                Expression<Func<TSource, Object>> keySelector = sortKey.Item1;
                string sortType = sortKey.Item2;
                string key = ExpressionHelper.GetMemberNameUnaryExpression(keySelector);
                queryable =  (ElasticQueryable<TSource>)queryable.OrderBy(key + " " + sortType);
            }
            //分页
            if (from > 0)
            {
                queryable = (ElasticQueryable<TSource>)queryable.Skip(from);
            }

            if (size > 0)
            {
                queryable = (ElasticQueryable<TSource>)queryable.Take(size);
            }

            var results =  queryable.ToList();
            int countR = results.Count();
            return results;
        }
    }
}