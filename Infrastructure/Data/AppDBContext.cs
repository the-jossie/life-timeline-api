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

    public DbSet<User> Users => Set<User>();

    public DbSet<Milestone> Milestones => Set<Milestone>();
}
