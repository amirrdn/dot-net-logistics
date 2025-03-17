using Logistics.Models;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentStatus> ShipmentStatuses { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShipmentStatus>()
                .HasOne(s => s.Shipment)
                .WithMany(s => s.ShipmentStatus)
                .HasForeignKey(s => s.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Shipment>()
                .HasIndex(s => s.AWB)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
