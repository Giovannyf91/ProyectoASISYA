using ProviderOptimizer.Domain.Entities;
using ProviderOptimizer.Infrastructure.DbContext;

namespace ProviderOptimizer.Infrastructure.Repositories
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly ProviderDbContext _context;

        public ProviderRepository(ProviderDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Provider>> GetAvailableProvidersAsync(string serviceType)
        {
            return await _context.Providers
                .Where(p => p.ServiceType == serviceType && p.Available)
                .ToListAsync();
        }

        public async Task UpdateProviderAsync(Provider provider)
        {
            _context.Providers.Update(provider);
            await _context.SaveChangesAsync();
        }
    }
}
