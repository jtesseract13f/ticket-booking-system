using ConcertAggregator.DTO;
using Refit;

namespace ConcertAggregator.BLL;

[ApiType("SimpleCinemaApi")]
public class CinemaNodeApi : INodeApi
{
    public async Task<IEnumerable<EventDto>> GetAllEvents(Uri baseUri)
    {
        var api = RestService.For<ISimpleCinemaApi>(baseUri.AbsoluteUri);
        var events = await api.GetCinemas();
        //TODO: Add resilience if API not available
        return events.Select(x => new EventDto(
            x.Id.ToString(),
            x.Cost,
            x.Place,
            x.Name,
            x.Description,
            x.StartDate,
            x.Uri
        ));
    }
}

public interface ISimpleCinemaApi
{
    [Get("/api/v1/cinemas")]
    Task<IEnumerable<SimpleCinemaEventDto>> GetCinemas();
}