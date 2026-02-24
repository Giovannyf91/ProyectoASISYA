namespace ProviderOptimizer.Domain.Entities
{
    /// <summary>
    /// Clase relacionada con las propiedades del proveedor
    /// </summary>
    public class Provider
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required string ServiceType { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public int Rating { get; set; }
        public bool Available { get; set; } = true;
    }
}