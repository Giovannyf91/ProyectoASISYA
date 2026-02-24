using Microsoft.EntityFrameworkCore;
using ProviderOptimizer.Application.Interfaces;
using ProviderOptimizer.Domain.Entities;
using ProviderOptimizer.Infrastructure.DbContext;

namespace ProviderOptimizerService.Infrastructure.Repositories
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
            try
            {
                return await _context.Providers
                    .Where(p => p.ServiceType == serviceType && p.Available)
                    .ToListAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new ApplicationException("Error al consultar proveedores disponibles.", dbEx);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al obtener proveedores.", ex);
            }
        }

        public async Task UpdateProviderAsync(Provider provider)
        {
            try
            {
                var existing = await _context.Providers.FindAsync(provider.Id);
                if (existing != null)
                {
                    _context.Entry(existing).CurrentValues.SetValues(provider);
                }
                else
                {
                    _context.Providers.Attach(provider);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException concEx)
            {
                throw new ApplicationException("Error de concurrencia al actualizar el proveedor.", concEx);
            }
            catch (DbUpdateException dbEx)
            {
                throw new ApplicationException("Error al actualizar el proveedor en la base de datos.", dbEx);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al actualizar el proveedor.", ex);
            }
        }
    }
}