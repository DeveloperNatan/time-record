using Microsoft.EntityFrameworkCore;
using TimeRecord.Models;

namespace TimeRecord.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<TimeRecords> TimeRecords { get; set; }
        public DbSet<Companies> Companies { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Matriculation)
                .IsUnique();

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

    }
}