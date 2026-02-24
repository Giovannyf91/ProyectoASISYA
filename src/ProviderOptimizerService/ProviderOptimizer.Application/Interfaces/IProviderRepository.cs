using ProviderOptimizer.Domain.Entities;

namespace ProviderOptimizer.Application.Interfaces
{    
    public interface IProviderRepository
    {
        Task<IEnumerable<Provider>> GetAvailableProvidersAsync(string serviceType);
        Task UpdateProviderAsync(Provider provider);
    }
}
