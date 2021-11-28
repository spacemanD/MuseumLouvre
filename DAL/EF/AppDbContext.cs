using DAL.EF.Entities;
using DAL.EF.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
            {
            }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Exhibit> Exhibits { get; set; }

        public DbSet<Collection> Collections { get; set; }

        public DbSet<PopAuthor> PopAuthors { get; set; }

        public DbSet<PopExhibit> PopExhibits { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
