using DAL.EF.Entities;
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

        public DbSet<Country> Countries { get; set; }

        public DbSet<Exhibit_Type> ExhibitTypes { get; set; }

        public DbSet<PopExhibit> PopExhibits { get; set; }

        public DbSet<PopCollection> PopCollections { get; set; }

        public DbSet<ArtDirections> ArtDirections { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
