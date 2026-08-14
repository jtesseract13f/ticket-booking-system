namespace ConcertAggregator.BLL;

public class ApiTypeAttribute(string apiType) : Attribute
{
    public string ApiType { get; init; } = apiType;
}