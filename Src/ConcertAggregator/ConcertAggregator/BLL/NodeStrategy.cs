using System.Reflection;

namespace ConcertAggregator.BLL;

public class NodeStrategy
{
    public Dictionary<string, INodeApi> NodeApis { get; init; }

    public NodeStrategy()
    {
       NodeApis = InitStrategy();
    }
    private Dictionary<string, INodeApi> InitStrategy()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes()
            .Where(x => x.GetInterfaces().Contains(typeof(INodeApi)));

        var createdNodes = new Dictionary<string, INodeApi>() { };
        foreach (var type in types)
        {
            try
            {
                var apiType = ((ApiTypeAttribute)type
                    .GetCustomAttribute(typeof(ApiTypeAttribute))).ApiType;
                createdNodes[apiType] = (INodeApi)type.GetConstructor(new Type[] { }).Invoke(new object?[] { });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        return createdNodes;
    }
}