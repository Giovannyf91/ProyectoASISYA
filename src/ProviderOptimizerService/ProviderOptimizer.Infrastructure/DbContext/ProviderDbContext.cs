using ProviderOptimizer.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ProviderOptimizer.Infrastructure.DbContext
{
    public class ProviderDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ProviderDbContext(DbContextOptions<ProviderDbContext> options)
            : base(options)
        {
        }

        // Tablas / DbSets
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración básica Provider
            modelBuilder.Entity<Provider>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.ServiceType).IsRequired();
                entity.Property(p => p.Rating).HasDefaultValue(0);
                entity.Property(p => p.Available).HasDefaultValue(true);
            });

            // Configuración básica Assignment
            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.AssignedAt).IsRequired();
                entity.HasOne<Provider>()      // Relación opcional
                      .WithMany()
                      .HasForeignKey(a => a.ProviderId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
