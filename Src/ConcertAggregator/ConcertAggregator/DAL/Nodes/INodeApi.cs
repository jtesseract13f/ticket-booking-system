using ConcertAggregator.DTO;

namespace ConcertAggregator.BLL;

public interface INodeApi
{
    Task<IEnumerable<EventDto>> GetAllEvents(Uri baseUri);
}