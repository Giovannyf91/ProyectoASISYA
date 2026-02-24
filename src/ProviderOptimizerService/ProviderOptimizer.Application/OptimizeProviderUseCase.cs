using ProviderOptimizer.Application.Interfaces;
using ProviderOptimizer.Domain.Entities;

namespace ProviderOptimizer.Application
{    
    public class OptimizeProviderUseCase
    {
        private readonly IProviderRepository _repository;

        public OptimizeProviderUseCase(IProviderRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Simula selección del mejor proveedor
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public async Task<Provider> GetOptimalProviderAsync(string serviceType, double lat, double lng)
        {
            var providers = await _repository.GetAvailableProvidersAsync(serviceType);
            
            var best = providers
                .OrderByDescending(p => p.Rating)
                .ThenBy(p => Distance(p.Lat, p.Lng, lat, lng))
                .FirstOrDefault();

            if (best != null)
            {
                best.Available = false;
                await _repository.UpdateProviderAsync(best);
            }

            return best;
        }

        /// <summary>
        /// Calculo de distancia
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lng1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        private double Distance(double lat1, double lng1, double lat2, double lng2)
        {            
            return Math.Sqrt(Math.Pow(lat1 - lat2, 2) + Math.Pow(lng1 - lng2, 2));
        }
    }
}
