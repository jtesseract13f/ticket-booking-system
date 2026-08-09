using ConcertAggregator.Infrastructure;
using ConcertAggregator.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcertAggregator.DAL.Repositories;

public class NodeRepository(ConcertDbContext _dbContext)
{
    public async Task<Guid> Add(Node node)
    {
        var entity = await _dbContext.AddAsync(node);
        await _dbContext.SaveChangesAsync();
        return entity.Entity.Id;
    }

    public async Task<Guid> Update(Node node)
    {
        var entity = _dbContext.Update(node);
        await _dbContext.SaveChangesAsync();
        return entity.Entity.Id;
    }

    public async Task<Guid> Delete(Guid id)
    {
        var entity = await GetById(id);
        _dbContext.Nodes.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<Node> GetById(Guid id)
    {
        var entity = await _dbContext.Nodes.FindAsync(id);
        if (entity == null) throw new EntityNotFoundException($"Node with id {id} not found");
        return entity;
    }

    public async Task<IEnumerable<Node>> GetAll()
    {
        return _dbContext.Nodes.AsNoTracking();
    }
}