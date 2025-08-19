using Microsoft.EntityFrameworkCore;
using Tmb.OrderManagementSystem.Core.Domain;

namespace Tmb.OrderManagementSystem.Core.Infra.Database;

public class TmbDbContext(DbContextOptions<TmbDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TmbDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}