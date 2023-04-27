using Nest;

namespace LinqToElasticSearch
{
    public abstract class Node
    {
        public abstract QueryContainer Accept(INodeVisitor visitor);
        public  bool IsSubQuery { get; set; }
        
        public  string SubQueryPath { get; set; }
        
        public  string SubQueryFullPath { get; set; }
    }
}