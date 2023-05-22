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
    public class GetLogEventAsyncTest: IntegrationTestsBase<SampleData>
    {
        private readonly ILinqQueryRepository<LogEventIndex, string> _blockIndexRepository;
        public GetLogEventAsyncTest()
        {
            _blockIndexRepository = new LinqQueryRepository<LogEventIndex, string>(new DefaultEsClientProvider());
        }
        public List<LogEventIndex> FilterNested(Func<QueryContainerDescriptor<LogEventIndex>, QueryContainer> filterFunc,string index)
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.67.164:9200"));
            var client = new ElasticClient(settings);
            Func<SearchDescriptor<LogEventIndex>, ISearchRequest> selector = new Func<SearchDescriptor<LogEventIndex>, ISearchRequest>(s => s
                .Index(index)
                .Query((Func<QueryContainerDescriptor<LogEventIndex>, QueryContainer>)(filterFunc ?? (q=>q.MatchAll())))
                .From(0)
                .Size(10000)
            );
            ISearchRequest search = selector(new SearchDescriptor<LogEventIndex>());
            var dsl = client.RequestResponseSerializer.SerializeToString(search);
            Console.WriteLine(dsl);
            
            var result = client.Search(selector);
            Console.WriteLine("totalCount:" + result.Total);
            return result.Documents.ToList();
        }

        
        [Test]
        public void GetLogEventAsyncTestByNestV1()
        {
            var index = "aelfindexer.logeventindex";
            var mustQuery = new List<Func<QueryContainerDescriptor<LogEventIndex>, QueryContainer>>();
            mustQuery.Add(q=>q.Term(i=>i.Field(f=>f.ChainId).Value("AELF")));
            mustQuery.Add(q => q.Range(i => i.Field(f => f.BlockHeight).GreaterThanOrEquals(0)));
            mustQuery.Add(q => q.Range(i => i.Field(f => f.BlockHeight).LessThanOrEquals(63468580)));
            mustQuery.Add(q=>q.Term(i=>i.Field(f=>f.Confirmed).Value(true)));
            
            var shouldQuery = new List<Func<QueryContainerDescriptor<LogEventIndex>, QueryContainer>>();
            
            var shouldMustQuery = new List<Func<QueryContainerDescriptor<LogEventIndex>, QueryContainer>>();
            shouldMustQuery.Add(s =>
                s.Match(i =>
                    i.Field(f=>f.ContractAddress).Query("pGa4e5hNGsgkfjEGm72TEvbF7aRDqKBd4LuXtab4ucMbXLcgJ")));
            
            var shouldMushShouldQuery = new List<Func<QueryContainerDescriptor<LogEventIndex>, QueryContainer>>();
            shouldMushShouldQuery.Add(s =>
                s.Match(i => i.Field(f=>f.EventName).Query("MiningInformationUpdated")));
            
            shouldMustQuery.Add(q => q.Bool(b => b.Should(shouldMushShouldQuery)));
            shouldQuery.Add(q => q.Bool(b => b.Must(shouldMustQuery)));
            
            mustQuery.Add(q => q.Bool(b => b.Should(shouldQuery)));
            QueryContainer Filter(QueryContainerDescriptor<LogEventIndex> f) => f.Bool(b => b.Must(mustQuery));
            var results = FilterNested(Filter, index);
            Console.WriteLine("queryCount:" + results.Count);
            foreach( var item in results){
                Console.WriteLine(item.ChainId);
                Console.WriteLine(item.BlockHeight);
                Console.WriteLine(item.TransactionId);
            }
        }
        [Test]
        public void GetLogEventAsyncTestByLinqV1()
        {
            var results = 
                _blockIndexRepository.
                    WhereClause(a=> a.ChainId == "tDVV" && a.BlockHeight >= 0 && a.BlockHeight <= 73597150 && a.Confirmed == true && (a.ContractAddress == "BNPFPPwQ3DE9rwxzdY61Q2utU9FZx9KYUnrYHQqCR6N4LLhUE" || a.EventName == "MiningInformationUpdated"))
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