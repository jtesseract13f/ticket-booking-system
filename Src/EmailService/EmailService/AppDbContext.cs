using ConcertAggregator.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcertAggregator.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Mailing> Mailings => Set<Mailing>();
    public DbSet<Filter> Filters => Set<Filter>();
    public DbSet<SentTicket> SentTickets => Set<SentTicket>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Mailing>(e =>
        {
            e.HasOne(m => m.Filter)
                .WithOne()
                .HasForeignKey<Mailing>("FilterId")
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(m => m.SentTickets)
                .WithOne()
                .HasForeignKey(st => st.MailingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<SentTicket>()
            .HasIndex(st => new { st.MailingId, st.TicketId })
            .IsUnique();
    }
}