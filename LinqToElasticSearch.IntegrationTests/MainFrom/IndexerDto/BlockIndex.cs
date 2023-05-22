using System.Collections.Generic;

namespace LinqToElasticSearch.IntegrationTests.MainFrom.IndexerDto;

public class BlockIndex:BlockBase
{
    // public List<Transaction> Transactions {get;set;}
    public List<string> TransactionIds { get; set; }
    public int LogEventCount { get; set; }
}