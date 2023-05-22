using System;
using System.Collections.Generic;
using LinqToElasticSearch.IntegrationTests.MainFrom.IndexerDto;
using LinqToElasticSearch.Provider;
using Nest;
using NUnit.Framework;

namespace LinqToElasticSearch.IntegrationTests.MainFrom.IndexerTests
{
    public class GetBlockCountTest: IntegrationTestsBase<SampleData>
    {
        private readonly ILinqQueryRepository<BlockIndex, string> _blockIndexRepository;
        public GetBlockCountTest()
        {
            _blockIndexRepository = new LinqQueryRepository<BlockIndex, string>(new DefaultEsClientProvider());
        }
        public long  FilterNested(Func<QueryContainerDescriptor<BlockIndex>, QueryContainer> filterFunc,string index)
        {
            var settings = new ConnectionSettings(new Uri("http://192.168.67.164:9200"));
            var client = new ElasticClient(settings);
            var result = client.Count<BlockIndex>(c => c.Index(index).Query(filterFunc));
            return result.Count;
        }

        [Test]
        public void GetBlockCountAsyncTestResultByNest()
        {
            var index = "aelfindexer.blockindex";
            var mustQuery = new List<Func<QueryContainerDescriptor<BlockIndex>, QueryContainer>>();
            List<BlockDto> items = new List<BlockDto>();
            mustQuery.Add(q => q.Term(i => i.Field(f => f.ChainId).Value("AELF")));
            mustQuery.Add(q => q.Range(i => i.Field(f => f.BlockHeight).GreaterThanOrEquals(0)));
            mustQuery.Add(q => q.Range(i => i.Field(f => f.BlockHeight).LessThanOrEquals(79510100)));
            mustQuery.Add(q => q.Term(i => i.Field(f => f.Confirmed).Value(true)));
            QueryContainer Filter(QueryContainerDescriptor<BlockIndex> f) => f.Bool(b => b.Must(mustQuery));
            var result = FilterNested(Filter, index);
            Console.WriteLine("queryCount:" + result);
        }

        [Test]
        public void GetBlockCountAsyncTestResultByLinq()
        {
            var results = _blockIndexRepository.Count(p =>
                (p.ChainId == "AELF" && p.BlockHeight >= 0 && p.BlockHeight <= 79510100 && p.Confirmed == true));
            Console.WriteLine("queryCount:" + results);
        }
    }
}