namespace ProviderOptimizer.Api.DTOs
{
    public class OptimizeRequest
    {
        public string ServiceType { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
