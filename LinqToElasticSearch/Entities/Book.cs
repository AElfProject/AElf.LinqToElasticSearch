using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace LinqToElasticSearch.Entities
{
    public class Book : IEntity<string>
    {
        public int Price { get; set; }

        public string Book_name { get; set; }

        public DateTime Publish_date { get; set; }

        public string Publisher { get; set; }

        public List<Authors> Authors { get; set; }
        public object[] GetKeys()
        {
            throw new NotImplementedException();
        }

        public string Id { get; }
    }

    public class Authors
    {
        public string Name { get; set; }

        [NestedAttributes("")] public int Age { get; set; }
    }
    
}