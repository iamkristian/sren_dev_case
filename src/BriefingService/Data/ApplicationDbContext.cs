using Microsoft.EntityFrameworkCore;
using BriefingService.Models;

namespace BriefingService.Data 
{
  public class ApplicationDbContext : DbContext 
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
    {
    }

    public DbSet<Briefing> Briefings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
    }
  }
}
