using Microsoft.EntityFrameworkCore;
using RedGraniteCms.Server.Core.Models;

namespace RedGraniteCms.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .HasIndex(e => e.Created);

            modelBuilder.Entity<Item>()
                .HasIndex(e => e.LastModified);
        }
    }
}
