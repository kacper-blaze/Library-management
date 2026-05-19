using lab10.Configuration;
using Microsoft.EntityFrameworkCore;

namespace lab10.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Login> Loginy { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Borrowing> Borrowings { get; set; }

    // This can safely remain as a fallback, but the constructor above 
    // is what prevents the exit code 134 crash.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(AppSettings.GetConfiguration().ConnectionString);
        }
    }
}