using System;
using System.Linq;
using LinqToElasticSearch.Provider;
using LinqToElasticSearch.Entities;
using NUnit.Framework;

namespace LinqToElasticSearch.IntegrationTests.MainFrom
{
    public class DemoTestV2: IntegrationTestsBase<SampleData>
    {

        private readonly ILinqQueryRepository<Book, string> _booksIndexRepository;
        public DemoTestV2()
        {
            _booksIndexRepository = new LinqQueryRepository<Book, string>(new DefaultEsClientProvider());;
        }

        [Test]
        public void YnTestResult()
        {
            var results = _booksIndexRepository.WhereClause(p =>
                    (p.Price >= 14 && p.Book_name.Contains("The")) && p.Authors.Any(a => a.Age >= 29 && a.Age <= 62))
                .OrderDesc(p => p.Price)
                .OrderASC(p => p.Publish_date)
                .Skip(0)
                .Take(10)
                .ToList();
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