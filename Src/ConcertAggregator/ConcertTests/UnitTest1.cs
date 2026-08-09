using ConcertAggregator.DAL;
using ConcertAggregator.DAL.Repositories;
using ConcertAggregator.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcertTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        
    }
    [Fact]
    public async Task Create_ShouldReturnSuccess_WhenDataGiven()
    {
        var options = new DbContextOptionsBuilder<ConcertDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var context = new ConcertDbContext(options))
        {
            var repo = new NodeRepository(context);
            var res = await repo.Add(new Node()
            {
                ApiType = "Honk",
                BaseUrl = "ff",
                IsBlocked = false
            });
            Assert.NotNull(res);
        }
    }
}