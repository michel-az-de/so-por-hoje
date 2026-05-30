using Microsoft.EntityFrameworkCore;
using SoPorHoje.Api.Data.Entities;

namespace SoPorHoje.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<PledgeEntity> Pledges => Set<PledgeEntity>();
    public DbSet<ChipEventEntity> ChipEvents => Set<ChipEventEntity>();
    public DbSet<ResetEventEntity> ResetEvents => Set<ResetEventEntity>();
    public DbSet<MeetingEntity> Meetings => Set<MeetingEntity>();
    public DbSet<ReflectionEntity> Reflections => Set<ReflectionEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Users
        modelBuilder.Entity<UserEntity>(e =>
        {
            e.HasIndex(u => u.DeviceId).IsUnique();
        });

        // Pledges — unique constraint por usuário e data
        modelBuilder.Entity<PledgeEntity>(e =>
        {
            e.HasIndex(p => new { p.UserId, p.PledgeDate }).IsUnique();
            e.HasOne(p => p.User)
             .WithMany(u => u.Pledges)
             .HasForeignKey(p => p.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ChipEvents
        modelBuilder.Entity<ChipEventEntity>(e =>
        {
            e.HasOne(c => c.User)
             .WithMany(u => u.ChipEvents)
             .HasForeignKey(c => c.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ResetEvents
        modelBuilder.Entity<ResetEventEntity>(e =>
        {
            e.HasOne(r => r.User)
             .WithMany(u => u.ResetEvents)
             .HasForeignKey(r => r.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Meetings
        modelBuilder.Entity<MeetingEntity>(e =>
        {
            e.HasIndex(m => m.IsActive)
             .HasFilter("is_active = TRUE");
        });

        // Reflections — date_key único
        modelBuilder.Entity<ReflectionEntity>(e =>
        {
            e.HasIndex(r => r.DateKey).IsUnique();
        });
    }
}
