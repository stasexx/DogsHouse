using Xunit;
using Application.Dogs;
using Application.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Tests.Tests.DogListTests;

public class DogsListTests
{
    private readonly DbContextOptions<DataContext> _dbContextOptions;

    public DogsListTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (var context = new DataContext(_dbContextOptions))
        {
            var dogs = new List<Dog>
            {
                new Dog { Name = "Rex", Color = "Brown", TailLength = 20, Weight = 30 },
                new Dog { Name = "Buddy", Color = "Black", TailLength = 15, Weight = 25 },
            };

            context.Dogs.AddRange(dogs);
            context.SaveChanges();
        }
    }

    [Fact]
    public async Task Handle_ShouldReturnSortedAndPaginatedDogs()
    {
        using (var context = new DataContext(_dbContextOptions))
        {
            var handler = new DogList.Handler(context);
            var request = new DogList.Command
            {
                AttributeName = "name",
                OrderType = "asc",
                PageNumber = 1,
                PageSize = 1
            };
            
            var result = await handler.Handle(request, CancellationToken.None);
            
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Buddy", result[0].Name);
        }
    }

    [Fact]
    public async Task Handle_ShouldReturnSortedDescendingAndPaginatedDogs()
    {
        using (var context = new DataContext(_dbContextOptions))
        {
            var handler = new DogList.Handler(context);
            var request = new DogList.Command
            {
                AttributeName = "name",
                OrderType = "desc",
                PageNumber = 1,
                PageSize = 1
            };
            
            var result = await handler.Handle(request, CancellationToken.None);
            
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Rex", result[0].Name);
        }
    }
    
    [Fact]
    public async Task Handle_ShouldReturnCorrectListOfDogs()
    {
        var dogs = new List<Dog>
        {
            new Dog { Name = "Dog1", Color = "Brown", TailLength = 20, Weight = 30 },
            new Dog { Name = "Dog2", Color = "Black", TailLength = 15, Weight = 25 },
        };

        var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var context = new DataContext(dbContextOptions))
        {
            context.Dogs.AddRange(dogs);
            await context.SaveChangesAsync();
        }

        var query = new DogList.Command { AttributeName = "name", OrderType = "asc", PageNumber = 1, PageSize = 10 };
        
        List<DogDto> result;
        using (var context = new DataContext(dbContextOptions))
        {
            var handler = new DogList.Handler(context);
            result = await handler.Handle(query, CancellationToken.None);
        }
        
        Assert.NotNull(result);
        Assert.Equal(dogs.Count, result.Count);
    }
}
