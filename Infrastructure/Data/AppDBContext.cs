using Microsoft.EntityFrameworkCore;
using LifeTimelineApi.Entities;

namespace LifeTimelineApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options
    ) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Milestone>()
            .HasOne(m => m.User)
            .WithMany(u => u.Milestones)
            .HasForeignKey(m => m.UserId);
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Milestone> Milestones => Set<Milestone>();
}
