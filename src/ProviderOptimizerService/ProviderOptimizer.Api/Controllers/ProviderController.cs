using Microsoft.AspNetCore.Mvc;
using ProviderOptimizer.Api.DTOs;
using ProviderOptimizer.Application;

namespace ProviderOptimizer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProviderController : ControllerBase
    {
        private readonly OptimizeProviderUseCase _optimizeUseCase;
        private readonly GetAvailableProvidersUseCase _availableUseCase;

        public ProviderController(OptimizeProviderUseCase optimizeUseCase, GetAvailableProvidersUseCase availableUseCase)
        {
            _optimizeUseCase = optimizeUseCase;
            _availableUseCase = availableUseCase;
        }

        [HttpPost("optimize")]
        public async Task<IActionResult> Optimize([FromBody] OptimizeRequest request)
        {
            var provider = await _optimizeUseCase.GetOptimalProviderAsync(
                request.ServiceType, request.Lat, request.Lng);

            if (provider == null) return NotFound();

            return Ok(provider);
        }

        [HttpGet("available")]
        public async Task<IActionResult> Available([FromQuery] string serviceType)
        {
            var providers = await _availableUseCase.ExecuteAsync(serviceType);
            return Ok(providers);
        }
    }
}