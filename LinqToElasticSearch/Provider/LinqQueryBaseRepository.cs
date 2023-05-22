using LinqToElasticSearch.Provider;
using Nest;
using Volo.Abp.Domain.Entities;

namespace LinqToElasticSearch.Provider
{
    public class LinqQueryBaseRepository<TEntity, TKey> 
        where TEntity : class, IEntity<TKey>, new()
    {
        private readonly IEsClientProvider _esClientProvider;
        protected virtual string IndexName { get; private set; }

        protected LinqQueryBaseRepository(IEsClientProvider esClientProvider, string index = null)
        {
            _esClientProvider = esClientProvider;
            IndexName = index ?? ("aelfindexer." + typeof(TEntity).Name.ToLower());
        }

        protected ElasticClient GetElasticClient()
        {
            return _esClientProvider.GetClient();
        }
    }
}