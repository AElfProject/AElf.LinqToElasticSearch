using System;
using System.Collections.Generic;
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
        [Test]
        public void YnTestResult()
        {
            Expression<Func<Books, bool>> expression = p =>
                (p.Price >= 14 && p.Book_name == "The Great Gatsby") || p.Authors.Any(a => a.Age >= 29 && a.Age <= 62);

            var results = Filter<Books>("book", expression);
            foreach (var result in results)
            {
                Console.WriteLine(result.Price);
                Console.WriteLine(result.Book_name);
                Console.WriteLine(result.Publisher);
                Console.WriteLine(result.PublishDate);
                Console.WriteLine(result.Authors[0].Name);
                Console.WriteLine(result.Authors[0].Age);
                Console.WriteLine("=================");

            }
        }
    }
}