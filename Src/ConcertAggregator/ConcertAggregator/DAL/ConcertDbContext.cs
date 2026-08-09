using ConcertAggregator.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcertAggregator.DAL;

public class ConcertDbContext(DbContextOptions<ConcertDbContext> options) : DbContext(options)
{
    public DbSet<Event> Events { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<EventTranslation> EventTranslations { get; set; }
    public DbSet<Node> Nodes { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}