using System;
using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;
using LinqToElasticSearch.IntegrationTests.MainFrom.IndexerDto;
using LinqToElasticSearch.Provider;
using Nest;
using NUnit.Framework;
using Volo.Abp.DependencyInjection;

namespace LinqToElasticSearch.IntegrationTests.MainFrom.IndexerTests
{
    public class GetBlockTest: IntegrationTestsBase<SampleData> ,ITransientDependency
    {
        private readonly ILinqQueryRepository<BlockIndex, string> _blockIndexRepository;
        public GetBlockTest()
        {
           _blockIndexRepository = new LinqQueryRepository<BlockIndex, string>(new DefaultEsClientProvider());
        }
        public List<BlockIndex> FilterNested(Func<QueryContainerDescriptor<BlockIndex>, QueryContainer> filterFunc,string index)
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.67.164:9200"));
            var client = new ElasticClient(settings);
            Func<SearchDescriptor<BlockIndex>, ISearchRequest> selector = new Func<SearchDescriptor<BlockIndex>, ISearchRequest>(s => s
                .Index(index)
                .Query((Func<QueryContainerDescriptor<BlockIndex>, QueryContainer>)(filterFunc ?? (q=>q.MatchAll())))
                .From(0)
                .Size(10)
            );
            ISearchRequest search = selector(new SearchDescriptor<BlockIndex>());
            var dsl = client.RequestResponseSerializer.SerializeToString(search);
            Console.WriteLine(dsl);
            
            var result = client.Search(selector);
            Console.WriteLine("totalCount:" + result.Total);
            return result.Documents.ToList();


        }

        
        [Test]
        public void GetBlockAsyncTestResultByNest()
        {
            var index = "aelfindexer.blockindex";
            var mustQuery = new List<Func<QueryContainerDescriptor<BlockIndex>, QueryContainer>>();
            List<BlockDto> items = new List<BlockDto>();
            mustQuery.Add(q => q.Term(i => i.Field(f => f.ChainId).Value("AELF")));
            mustQuery.Add(q => q.Range(i => i.Field(f => f.BlockHeight).GreaterThanOrEquals(0)));
            mustQuery.Add(q => q.Range(i => i.Field(f => f.BlockHeight).LessThanOrEquals(79510100)));
            mustQuery.Add(q => q.Term(i => i.Field(f => f.Confirmed).Value(true)));
            QueryContainer Filter(QueryContainerDescriptor<BlockIndex> f) => f.Bool(b => b.Must(mustQuery));
            var results = FilterNested(Filter, index);
            Console.WriteLine("queryCount:" + results.Count);
            foreach( var item in results){
                Console.WriteLine(item.ChainId);
                Console.WriteLine(item.BlockHeight);
            }
        }

        
        [Test]
        public void GetBlockAsyncTestResultByLinq()
        {
            var results = _blockIndexRepository.WhereClause(p =>
                    (p.ChainId == "AELF" && p.BlockHeight >= 0 && p.BlockHeight <= 79510100 && p.Confirmed == true))
                .OrderDesc(p => p.BlockHeight)
                .Skip(0)
                .Take(10)
                .ToList();
            Console.WriteLine("queryCount:" + results.Count);
            foreach( var item in results){
                Console.WriteLine(item.ChainId);
                Console.WriteLine(item.BlockHeight);
            }
        }
    }
   
}