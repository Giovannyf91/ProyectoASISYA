using ProviderOptimizer.Application.Interfaces;
using ProviderOptimizer.Domain.Entities;

namespace ProviderOptimizer.Application
{
    public class GetAvailableProvidersUseCase
    {
        private readonly IProviderRepository _repository;

        public GetAvailableProvidersUseCase(IProviderRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Provider>> ExecuteAsync(string serviceType)
        {
            return _repository.GetAvailableProvidersAsync(serviceType);
        }
    }
}