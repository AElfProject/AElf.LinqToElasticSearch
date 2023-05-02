using System;
using System.Collections.Generic;

namespace LinqToElasticSearch
{
    public class Books
    {
        public decimal Price { get; set; }

        public string Book_name { get; set; }

        public DateTime Publish_date { get; set; }

        public string Publisher { get; set; }

        public List<Authors> Authors { get; set; }
    }

    public class Authors
    {
        public string Name { get; set; }

        [NestedAttributes("")] public int Age { get; set; }
    }
    
}