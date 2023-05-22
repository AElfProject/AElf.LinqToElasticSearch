using System;
using Nest;

namespace LinqToElasticSearch
{
    public abstract class Node: ICloneable
    {
        public abstract QueryContainer Accept(INodeVisitor visitor);
        public  bool IsSubQuery { get; set; }
        public  string SubQueryPath { get; set; }
        public  string SubQueryFullPath { get; set; }
        public  bool  ParentIsSubQuery { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}