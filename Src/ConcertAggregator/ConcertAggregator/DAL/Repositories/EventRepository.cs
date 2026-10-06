using ConcertAggregator.DAL;
using ConcertAggregator.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcertAggregator.BLL;

public class EventRepository(ConcertDbContext _dbContext)
{
    public async Task<Guid> AddEvent(Event entity)
    {
        entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
        await _dbContext.Events.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<Guid> UpdateEvent(Event entity)
    {
        _dbContext.Events.Update(entity);
        await _dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<Guid> DeleteEvent(Guid id)
    {
        var entity = await _dbContext.Events
            .Include(e => e.EventTranslations)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entity is null)
            throw new KeyNotFoundException($"Event {id} not found");

        _dbContext.Events.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return id;
    }

    public async Task<Event?> GetEventById(Guid id)
    {
        return await _dbContext.Events
            .Include(e => e.EventTranslations)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Event>> GetAllEventsForNode(Guid nodeId)
    {
        return await _dbContext.Events
            .AsNoTracking()
            .Include(e => e.EventTranslations)
            .Where(e => e.Node.Id == nodeId)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Event> Items, int Total)> GetPagination(
        int skip, int take, int languageId = 1)
    {
        var query = _dbContext.Events
            .AsNoTracking()
            .Where(e => e.EventTranslations.Any(t => t.Language.Id == languageId))
            .OrderBy(e => e.StartDate);

        var total = await query.CountAsync();

        var items = await query
            .Skip(skip)
            .Take(take)
            .Include(e => e.EventTranslations.Where(t => t.Language.Id == languageId))
            .ToListAsync();

        return (items, total);
    }
    
    public async Task<Event?> GetByUriAsync(string uri, Guid nodeId, CancellationToken ct = default)
    {
        return await _dbContext.Events
            .Include(e => e.EventTranslations).ThenInclude(t => t.Language)
            .FirstOrDefaultAsync(e => e.Uri == uri && e.Node.Id == nodeId, ct);
    }

    // ---------- TRANSLATION ----------

    public async Task<int> AddTranslation(EventTranslation translation)
    {
        await _dbContext.EventTranslations.AddAsync(translation);
        await _dbContext.SaveChangesAsync();
        return translation.Id;
    }

    public async Task UpdateTranslation(EventTranslation translation)
    {
        _dbContext.EventTranslations.Update(translation);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteTranslation(int id)
    {
        var entity = await _dbContext.EventTranslations.FindAsync(id);
        if (entity is null) return;

        _dbContext.EventTranslations.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<EventTranslation?> GetTranslation(int id)
    {
        return await _dbContext.EventTranslations
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}