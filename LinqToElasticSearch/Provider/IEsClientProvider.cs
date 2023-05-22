using System;
using Microsoft.Extensions.Configuration;
using Nest;
using Volo.Abp.DependencyInjection;

namespace LinqToElasticSearch.Provider
{
    public interface IEsClientProvider
    {
        ElasticClient GetClient();
    }
    
    public class DefaultEsClientProvider: IEsClientProvider, ISingletonDependency
    {
        private readonly Lazy<ElasticClient> _client;
        
        public DefaultEsClientProvider()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("/Users/yanfeng/Desktop/Code/linq2/AElf.LinqToElasticSearch/LinqToElasticSearch/Config/appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            var elasticsearchUri = configuration["ElasticsearchUri"];           
            var settings = new ConnectionSettings(new Uri(elasticsearchUri));
            _client = new Lazy<ElasticClient>(() => new ElasticClient(settings));
        }
        public ElasticClient GetClient()
        {
            return _client.Value;
        }
    }
}