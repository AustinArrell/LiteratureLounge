using Microsoft.EntityFrameworkCore;
using LiteratureLounge.Models;

namespace Purrs_And_Prose.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Book> Books { get; set; }
    }
}
