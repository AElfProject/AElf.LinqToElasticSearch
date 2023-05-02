using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Nest;
using NUnit.Framework;

namespace LinqToElasticSearch.IntegrationTests.MainFrom
{
    public class DemoTest: IntegrationTestsBase<SampleData>
    {


        public static List<T> Filter<T>(string index, Expression<Func<T, bool>> predicate)
        {
            var client = new ElasticClient(new Uri("http://localhost:9200"));
            var queryable = new ElasticQueryable<T>(client, index).Where(predicate.ToString());
            var results =  queryable.ToList();
            int countR = results.Count();
            return results;

        }

        public static List<TSource> FilterOrderBy<TSource,TKey>(string index, Expression<Func<TSource, bool>> where, List<Tuple<Expression<Func<TSource, object>>,string>> sortKeys, int from, int size)
        {
            var client = new ElasticClient(new Uri("http://localhost:9200"));
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
        [Test]
        public void YnTestResult()
        {
            //构造查询表达式
            Expression<Func<Books, bool>> where = p =>
                (p.Price >= 14 && p.Book_name.Contains("The")) && p.Authors.Any(a => a.Age >= 29 && a.Age <= 62);
            //构造排序参数
            var sortedBooks = new List<Tuple<Expression<Func<Books, Object>>, string>>()
            {
                Tuple.Create<Expression<Func<Books, Object>>, string>(b => b.Price, "DESC"),
                Tuple.Create<Expression<Func<Books, Object>>, string>(b => b.Publish_date, "ASC")
            };
            //执行查询
            var client = new ElasticClient(new Uri("http://localhost:9200"));
            var results = LinqToElasticSearchUtil.Query<Books,Object>(client, "book", where, sortedBooks, 0, 10);
            foreach (var result in results)
            {
                Console.WriteLine(result.Price);
                Console.WriteLine(result.Book_name);
                Console.WriteLine(result.Publisher);
                Console.WriteLine(result.Publish_date);
                Console.WriteLine(result.Authors[0].Name);
                Console.WriteLine(result.Authors[0].Age);
                Console.WriteLine("=================");

            }
        }
    }
}