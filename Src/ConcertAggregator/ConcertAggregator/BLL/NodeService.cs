using ConcertAggregator.DAL;
using ConcertAggregator.DAL.Repositories;
using ConcertAggregator.DTO;
using ConcertAggregator.Models;

namespace ConcertAggregator.BLL;

public class NodeService(NodeRepository _repository)
{
    public async Task<Guid> AddNode(AddNodeDto nodeDto)
    {
        return await _repository.Add(new Node()
        {
            ApiType = nodeDto.ApiType,
            BaseUrl = nodeDto.BaseUrl,
            IsBlocked = false
        });
    }

    public async Task<Guid> DeleteNode(DeleteNodeDto nodeDto)
    {
        return await _repository.Delete(nodeDto.Id);
    }

    public async Task<Guid> UpdateNode(UpdateNodeDto nodeDto)
    {
        var entity = await _repository.GetById(nodeDto.Id);
        entity.ApiType = nodeDto.ApiType ?? entity.ApiType;
        entity.BaseUrl = nodeDto.BaseUrl ?? entity.BaseUrl;
        entity.IsBlocked = nodeDto.IsBlocked ?? entity.IsBlocked;
        return await _repository.Update(entity);
    }

    public async Task<IEnumerable<GetNodeDto>> GetAllNodes()
    {
        return (await _repository.GetAll()).Select(x => new GetNodeDto(x.Id, x.ApiType, x.BaseUrl, x.IsBlocked));
    }
}