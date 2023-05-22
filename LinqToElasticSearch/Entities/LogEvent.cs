using System;
using System.Collections.Generic;
using Nest;
namespace LinqToElasticSearch.Entities
{

    [NestedAttributes("LogEvents")]

    public class LogEvent : IBlockchainData
    {
        [Keyword] 
        // [NestedAttributes("")] 
        public string ChainId { get; set; }

        [Keyword] public string BlockHash { get; set; }

        [Keyword] public string PreviousBlockHash { get; set; }
        public long BlockHeight { get; set; }

        public DateTime BlockTime { get; set; }
        [Keyword] public string TransactionId { get; set; }

        [Keyword] public string ContractAddress { get; set; }

        [Keyword] public string EventName { get; set; }

        /// <summary>
        /// The ranking position of the event within the transaction
        /// </summary>
        public int Index { get; set; }

        public bool Confirmed { get; set; }

        public Dictionary<string, string> ExtraProperties { get; set; }
    }

    public interface IBlockchainData
    {
        string ChainId {get;set;}
        string BlockHash { get; set; }
        string PreviousBlockHash { get; set; }
        long BlockHeight { get; set; }
        DateTime BlockTime{get;set;}
        bool Confirmed{get;set;}
    }
}