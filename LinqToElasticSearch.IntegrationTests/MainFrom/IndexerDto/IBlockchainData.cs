using System;

namespace LinqToElasticSearch.IntegrationTests.MainFrom.IndexerDto;

public interface IBlockchainData
{
    string ChainId {get;set;}
    string BlockHash { get; set; }
    string PreviousBlockHash { get; set; }
    long BlockHeight { get; set; }
    DateTime BlockTime{get;set;}
    bool Confirmed{get;set;}
}