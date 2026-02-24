using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProviderOptimizer.Api.Controllers;
using ProviderOptimizer.Api.DTOs;
using ProviderOptimizer.Application;
using ProviderOptimizer.Domain.Entities;
using ProviderOptimizer.Infrastructure.DbContext;
using ProviderOptimizerService.Infrastructure.Repositories;
using Xunit;

namespace ProviderOptimizerTests
{
    public class ProviderControllerTests
    {
        // Método helper para crear una base de datos en memoria
        private ProviderDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ProviderDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ProviderDbContext(options);

            // Seed data
            context.Providers.AddRange(
                new Provider { Id = Guid.NewGuid(), Name = "Grua1", ServiceType = "Grua", Available = true, Lat = -34.6037, Lng = -58.3816, Rating = 5 },
                new Provider { Id = Guid.NewGuid(), Name = "Grua2", ServiceType = "Grua", Available = true, Lat = -34.61, Lng = -58.39, Rating = 4 },
                new Provider { Id = Guid.NewGuid(), Name = "Cerrajeria1", ServiceType = "Cerrajeria", Available = true, Lat = -34.62, Lng = -58.40, Rating = 5 }
            );
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task Endpoints_ShouldReturnExpectedResults()
        {
            var context = GetInMemoryDb();
            var repository = new ProviderRepository(context);
          
            var optimizeUseCase = new OptimizeProviderUseCase(repository);
            var availableUseCase = new GetAvailableProvidersUseCase(repository);
           
            var controller = new ProviderController(optimizeUseCase, availableUseCase);

            // --- Test POST /optimize ---
            var optimizeRequest = new OptimizeRequest { ServiceType = "Grua", Lat = -34.6037, Lng = -58.3816 };
            var optimizeResult = await controller.Optimize(optimizeRequest);
            var okOptimize = Assert.IsType<OkObjectResult>(optimizeResult);
            var optimalProvider = Assert.IsType<Provider>(okOptimize.Value);
            Assert.Equal("Grua1", optimalProvider.Name);          
        }
    }
}