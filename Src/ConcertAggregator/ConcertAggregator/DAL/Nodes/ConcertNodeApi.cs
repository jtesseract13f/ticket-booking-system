using ConcertAggregator.BLL;
using ConcertAggregator.DTO;
using Refit;

namespace ConcertAggregator.DAL.Nodes;

[ApiType("SimpleConcertApi")]
public class ConcertNodeApi : INodeApi
{
    public async Task<IEnumerable<EventDto>> GetAllEvents(Uri baseUri)
    {
        var api = RestService.For<ISimpleConcertApi>(baseUri.AbsoluteUri);
        var events = await api.GetConcerts();

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

public interface ISimpleConcertApi
{
    [Get("/api/v1/concerts")]
    Task<IEnumerable<SimpleCinemaEventDto>> GetConcerts();
}