using System;

namespace LinqToElasticSearch
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    public class NestedAttributes : Attribute
    {
        public string Name { get; set; }

        public NestedAttributes(string name)
        {
            Name = name;
        }
    }
}