using ConcertAggregator.DTO;

namespace ConcertAggregator.BLL;

public class EventService(NodeService _nodeService, NodeStrategy _nodeStrategy)
{
    //Задача - стащить все билеты из доступных узлов
    // Пока только все
    // Стащить все доступные ноды
    // Создать стратегию
    // пройтись по всем нодам и запустить стратегию
    //TODO: добавить обработку ошибки, если нода не доступна
    //TODO: добавить логику для перевода и сохранения событий
    //TODO: languageMiddleware А КАК
    public async Task<IEnumerable<EventDto>> GetAllEvents()
    {
        var nodes = await _nodeService.GetAllNodes();
        var allEvents = new List<EventDto>();
        foreach (var node in nodes)
        {
            try
            {
                var events = await _nodeStrategy.NodeApis[node.ApiType].GetAllEvents(new Uri(node.BaseUrl));
                allEvents.AddRange(events);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        return allEvents;
    }
    
}