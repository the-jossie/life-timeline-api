using Microsoft.EntityFrameworkCore;
using LifeTimelineApi.Entities;

namespace LifeTimelineApi.Data;

public class DBContext : DbContext
{
    public DBContext(
        DbContextOptions<DBContext> options
    ) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Milestone> Milestones => Set<Milestone>();
}
