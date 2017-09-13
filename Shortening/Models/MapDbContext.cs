using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shortening.Models
{
    public class MapDbContext : DbContext
    {
        public DbSet<MappedItem> Items { get; set; }

        public MapDbContext(DbContextOptions<MapDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MappedItem>().HasIndex(nameof(MappedItem.Alias));

            base.OnModelCreating(modelBuilder);
        }

    }

    public class DesignTimeMapDbContextFactory : IDesignTimeDbContextFactory<MapDbContext>
    {
        public MapDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<MapDbContext> builder = new DbContextOptionsBuilder<MapDbContext>()
                .UseSqlite("Filename=db.sqlite");
            return new MapDbContext(builder.Options);
        }
    }
}
