using LiteratureLounge.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Purrs_And_Prose.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookGenre>()
                .HasKey(bg => new { bg.BookId, bg.GenreId });
            modelBuilder.Entity<BookGenre>()
                .HasOne(bc => bc.Book)
                .WithMany(b => b.BookGenres)
                .HasForeignKey(bc => bc.BookId);
            modelBuilder.Entity<BookGenre>()
                .HasOne(bc => bc.Genre)
                .WithMany(c => c.BookGenres)
                .HasForeignKey(bc => bc.GenreId);

            modelBuilder.Entity<UserPreferencesBookIndexColumn>()
                .HasKey(up => new { up.UserPreferencesId, up.IndexColumnId });
            modelBuilder.Entity<UserPreferencesBookIndexColumn>()
                .HasOne(up => up.UserPreferences)
                .WithMany(up => up.UserPreferencesBookIndexColumns)
                .HasForeignKey(bc => bc.UserPreferencesId);
            modelBuilder.Entity<UserPreferencesBookIndexColumn>()
                .HasOne(up => up.IndexColumn)
                .WithMany(up => up.UserPreferencesBookIndexColumns)
                .HasForeignKey(bc => bc.IndexColumnId);
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }
        public DbSet<IndexColumn> IndexColumns { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }
        public DbSet<UserPreferencesBookIndexColumn> UserPreferenceIndexColumns { get; set; }
    }
}
