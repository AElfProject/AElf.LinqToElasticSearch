using System;
using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;
using LinqToElasticSearch.IntegrationTests.MainFrom.IndexerDto;
using LinqToElasticSearch.Provider;
using Nest;
using NUnit.Framework;
namespace LinqToElasticSearch.IntegrationTests.MainFrom.IndexerTests
{
    public class GetTransactionsAsyncTest: IntegrationTestsBase<SampleData>
    {
        private readonly ILinqQueryRepository<TransactionIndex, string> _transactionIndexRepository;
        public GetTransactionsAsyncTest()
        {
            _transactionIndexRepository = new LinqQueryRepository<TransactionIndex, string>(new DefaultEsClientProvider());
        }
        public List<TransactionIndex> FilterNested(Func<QueryContainerDescriptor<TransactionIndex>, QueryContainer> filterFunc,string index)
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.67.164:9200"));
            var client = new ElasticClient(settings);
            Func<SearchDescriptor<TransactionIndex>, ISearchRequest> selector = new Func<SearchDescriptor<TransactionIndex>, ISearchRequest>(s => s
                .Index(index)
                .Query((Func<QueryContainerDescriptor<TransactionIndex>, QueryContainer>)(filterFunc ?? (q=>q.MatchAll())))
                .From(0)
                .Size(10000)
            );
            ISearchRequest search = selector(new SearchDescriptor<TransactionIndex>());
            var dsl = client.RequestResponseSerializer.SerializeToString(search);
            Console.WriteLine(dsl);
            
            var result = client.Search(selector);
            Console.WriteLine("totalCount:" + result.Total);
            return result.Documents.ToList();
        }

        
        [Test]
        public void GetTransactionsAsyncTestByNestV1()
        {
            var index = "aelfindexer.transactionindex";
            var mustQuery = new List<Func<QueryContainerDescriptor<TransactionIndex>, QueryContainer>>();
            mustQuery.Add(q=>q.Term(i=>i.Field("LogEvents.chainId").Value("tDVV")));
            mustQuery.Add(q => q.Range(i => i.Field("LogEvents.blockHeight").GreaterThanOrEquals(0)));
            mustQuery.Add(q => q.Range(i => i.Field("LogEvents.blockHeight").LessThanOrEquals(73597150)));
            mustQuery.Add(q=>q.Term(i=>i.Field("LogEvents.confirmed").Value(true)));

            var shouldQuery = new List<Func<QueryContainerDescriptor<TransactionIndex>, QueryContainer>>();
            
            var shouldMustQuery = new List<Func<QueryContainerDescriptor<TransactionIndex>, QueryContainer>>();
            shouldMustQuery.Add(s =>
                s.Match(i =>
                    i.Field("LogEvents.contractAddress").Query("BNPFPPwQ3DE9rwxzdY61Q2utU9FZx9KYUnrYHQqCR6N4LLhUE")));

            var shouldMushShouldQuery = new List<Func<QueryContainerDescriptor<TransactionIndex>, QueryContainer>>();
            shouldMushShouldQuery.Add(s =>
                s.Match(i => i.Field("LogEvents.eventName").Query("MiningInformationUpdated")));
            if (shouldMushShouldQuery.Count > 0)
            {
                shouldMustQuery.Add(q => q.Bool(b => b.Should(shouldMushShouldQuery)));
            }
            shouldQuery.Add(q => q.Bool(b => b.Must(shouldMustQuery)));
            
            mustQuery.Add(q=>q.Bool(b=>b.Should(shouldQuery)));

            
            QueryContainer Filter(QueryContainerDescriptor<TransactionIndex> f) => f.Nested(q => q.Path("LogEvents")
                .Query(qq => qq.Bool(b => b.Must(mustQuery))));
            
            var results = FilterNested(Filter, index);
            Console.WriteLine("queryCount:" + results.Count);
            foreach( var item in results){
                Console.WriteLine(item.ChainId);
                Console.WriteLine(item.BlockHeight);
                Console.WriteLine(item.TransactionId);
            }
        }

        
        [Test]
        public void GetTransactionsAsyncTestByLinqV1()
        {
            var results = 
                _transactionIndexRepository.
                    WhereClause(p =>
                    p.LogEvents.Any(a=> a.ChainId == "tDVV" && a.BlockHeight >= 0 && a.BlockHeight <= 73597150 && a.Confirmed == true && (a.ContractAddress == "BNPFPPwQ3DE9rwxzdY61Q2utU9FZx9KYUnrYHQqCR6N4LLhUE" || a.EventName == "MiningInformationUpdated")))
                .Skip(0)
                .Take(1000)
                .ToList();
            Console.WriteLine("queryCount:" + results.Count);
            foreach( var item in results){
                Console.WriteLine(item.ChainId);
                Console.WriteLine(item.BlockHeight);

            }
        }
    }
   
}