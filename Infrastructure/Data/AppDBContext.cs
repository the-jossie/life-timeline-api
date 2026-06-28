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

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<MilestoneTag> MilestoneTags => Set<MilestoneTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Milestone>()
            .HasOne(m => m.User)
            .WithMany(u => u.Milestones)
            .HasForeignKey(m => m.UserId);

        modelBuilder.Entity<MilestoneTag>()
            .HasKey(x => new { x.MilestoneId, x.TagId });

        modelBuilder.Entity<MilestoneTag>()
            .HasOne(x => x.Milestone)
            .WithMany(x => x.MilestoneTags)
            .HasForeignKey(x => x.MilestoneId);

        modelBuilder.Entity<MilestoneTag>()
            .HasOne(x => x.Tag)
            .WithMany(x => x.MilestoneTags)
            .HasForeignKey(x => x.TagId);

        modelBuilder.Entity<Milestone>()
        .HasIndex(m => m.UserId);

        modelBuilder.Entity<Milestone>()
            .HasIndex(m => m.Date);

        modelBuilder.Entity<Milestone>()
            .HasIndex(m => m.Title);

        modelBuilder.Entity<Milestone>()
            .HasIndex(m => m.Description);

        modelBuilder.Entity<Milestone>()
            .HasIndex(m => m.Mood);

        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();

        modelBuilder.Entity<MilestoneTag>()
            .HasIndex(mt => new { mt.MilestoneId, mt.TagId });
    }

}
