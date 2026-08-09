using ConcertAggregator.DAL;
using Microsoft.EntityFrameworkCore;

namespace ConcertAggregator.BLL;

public class EventRepository(ConcertDbContext _dbContext)
{
    public async Task<Guid> AddTranslation()
    {
        return Guid.Empty;
    }

    public async Task UpdateTranslation()
    {
    }

    public async Task AddEvent()
    {
        
    }

    public async Task DeleteEvent()
    {
        
    }

    public async Task GetAllEventsForNode(Guid nodeId)
    {
        
    }

    public async Task GetPagination(int skip, int take, int languageId = 1)
    {
        
    }
    
}