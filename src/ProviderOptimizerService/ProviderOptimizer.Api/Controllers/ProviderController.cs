using Microsoft.AspNetCore.Mvc;

namespace ProviderOptimizer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProviderController : ControllerBase
    {
        private readonly OptimizeProviderUseCase _useCase;

        public ProviderController(OptimizeProviderUseCase useCase)
        {
            _useCase = useCase;
        }

        [HttpPost("optimize")]
        public async Task<IActionResult> Optimize([FromBody] OptimizeRequest request)
        {
            var provider = await _useCase.GetOptimalProviderAsync(request.ServiceType, request.Lat, request.Lng);
            if (provider == null) return NotFound();
            return Ok(provider);
        }

        [HttpGet("available")]
        public async Task<IActionResult> Available([FromQuery] string serviceType)
        {
            var providers = await _useCase._repository.GetAvailableProvidersAsync(serviceType);
            return Ok(providers);
        }
    }

    public class OptimizeRequest
    {
        public string ServiceType { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
