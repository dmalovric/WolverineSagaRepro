using Microsoft.EntityFrameworkCore;
using WolverineSagaIssue.GuidId;
using WolverineSagaIssue.StringId;

namespace WolverineSagaIssue;

public class LocalOrdersEfContext(DbContextOptions<LocalOrdersEfContext> options) : DbContext(options)
{
    public DbSet<LocalOrderWithGuidId> GuidIdOrders => Set<LocalOrderWithGuidId>();
    public DbSet<LocalOrderWithStringId> StringIdOrders => Set<LocalOrderWithStringId>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LocalOrderWithGuidId>()
            .HasKey(e => e.Id);
        modelBuilder.Entity<LocalOrderWithGuidId>()
            .Property(e => e.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<LocalOrderWithStringId>()
            .HasKey(e => e.Id);
        modelBuilder.Entity<LocalOrderWithStringId>()
            .Property(e => e.Id)
            .ValueGeneratedNever();
    }
}
