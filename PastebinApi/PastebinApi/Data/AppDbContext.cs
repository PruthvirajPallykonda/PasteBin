using Microsoft.EntityFrameworkCore;
using PastebinApi.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PastebinApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Paste> Pastes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paste>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Content).IsRequired();
            });
        }
    }
}
