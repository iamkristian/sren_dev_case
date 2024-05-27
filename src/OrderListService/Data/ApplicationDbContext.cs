using Microsoft.EntityFrameworkCore;
using OrderListService.Models;

namespace OrderListService.Data 
{
  public class ApplicationDbContext : DbContext 
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
    {
    }

    public DbSet<OrderList> OrderLists { get; set; }
    public DbSet<Asset> Assets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<OrderList>()
        .HasMany(o => o.Assets)
        .WithOne(a => a.OrderList)
        .HasForeignKey(a => a.OrderListId);
    }
  }
}
