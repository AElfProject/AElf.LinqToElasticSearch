using System;

namespace LinqToElasticSearch
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NestedAttributes : Attribute
    {
        public string Description { get; set; }
        public NestedAttributes(string description)
        {
            Description = description;
        }
    }
}