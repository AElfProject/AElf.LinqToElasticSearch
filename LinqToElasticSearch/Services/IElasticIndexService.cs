using LinqToElasticSearch.Provider;
using Microsoft.Extensions.Logging;
using Nest;
using Volo.Abp.DependencyInjection;

namespace LinqToElasticSearch.Services
{
    public interface IElasticIndexService
    {
        
    }

    public class ElasticIndexService : IElasticIndexService, ITransientDependency
    {
        private readonly IEsClientProvider _esClientProvider;
        private readonly ILogger<ElasticIndexService> _logger;

        public ElasticIndexService(IEsClientProvider esClientProvider, ILogger<ElasticIndexService> logger)
        {
            _esClientProvider = esClientProvider;
            _logger = logger;
        }
        private ElasticClient GetEsClient()
        {
            return _esClientProvider.GetClient();
        }
        
        
    }
}