using ProviderOptimizer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProviderOptimizer.Infrastructure.DbContext
{
    public class ProviderDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ProviderDbContext(DbContextOptions<ProviderDbContext> options)
            : base(options)
        {
        }

        public DbSet<Provider> Providers { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Provider>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.ServiceType).IsRequired();
                entity.Property(p => p.Rating).HasDefaultValue(0);
                entity.Property(p => p.Available).HasDefaultValue(true);
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.AssignedAt).IsRequired();
                entity.HasOne<Provider>()
                      .WithMany()
                      .HasForeignKey(a => a.ProviderId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
