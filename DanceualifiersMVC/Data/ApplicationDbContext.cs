using DanceualifiersMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DanceualifiersMVC.Data;

public class   ApplicationDbContext  : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
        
    public DbSet<Direction> Directions { get; set; }
    public DbSet<TimeSlot> TimeSlots { get; set; }
    public DbSet<Registration> Registrations { get; set; }
        
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
            
        // Configure relationships
        builder.Entity<Direction>()
            .HasOne(d => d.CreatedBy)
            .WithMany()
            .HasForeignKey(d => d.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
                
        builder.Entity<TimeSlot>()
            .HasOne(ts => ts.Direction)
            .WithMany(d => d.TimeSlots)
            .HasForeignKey(ts => ts.DirectionId)
            .OnDelete(DeleteBehavior.Cascade);
                
        builder.Entity<Registration>()
            .HasOne(r => r.User)
            .WithMany(u => u.Registrations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
                
        builder.Entity<Registration>()
            .HasOne(r => r.TimeSlot)
            .WithMany(ts => ts.Registrations)
            .HasForeignKey(r => r.TimeSlotId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}