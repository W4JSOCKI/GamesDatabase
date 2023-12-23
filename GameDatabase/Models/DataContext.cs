using Microsoft.EntityFrameworkCore;

namespace GameDatabase.Models;

public class DataContext : DbContext {
    private readonly IConfiguration _configuration;

    public DataContext(IConfiguration configuration) {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite(_configuration.GetConnectionString("Sqlite"));
    }

    public DbSet<Game> Games {get; set;}
    public DbSet<Developer> Developers {get; set;}
}