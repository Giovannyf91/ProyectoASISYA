using Microsoft.EntityFrameworkCore;
using ProviderOptimizer.Application;
using ProviderOptimizer.Application.Interfaces;
using ProviderOptimizer.Infrastructure.DbContext;
using ProviderOptimizerService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Conexión a PostgreSQL
builder.Services.AddDbContext<ProviderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Repositorios
builder.Services.AddScoped<IProviderRepository, ProviderRepository>();

// Use Cases
builder.Services.AddScoped<OptimizeProviderUseCase>();
builder.Services.AddScoped<GetAvailableProvidersUseCase>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
