using Application.Dogs;
using Application.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Xunit;

namespace Tests.Tests.CreateTests;

public class CreateDogTests
{
    private DbContextOptions<DataContext> _dbContextOptions;

    public CreateDogTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }
    
    [Fact]
    public async Task Handle_ShouldCreateDog_WhenValidRequest()
    {
        var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dogToCreate = new Dog { Name = "Dog1", Color = "Brown", TailLength = 20, Weight = 30 };
        var command = new Create.Command { Dog = dogToCreate };
        
        using (var context = new DataContext(dbContextOptions))
        {
            var handler = new Create.Handler(context);
            await handler.Handle(command, CancellationToken.None);
        }
        
        using (var context = new DataContext(dbContextOptions))
        {
            var createdDog = await context.Dogs.FirstOrDefaultAsync(d => d.Name == dogToCreate.Name);
            Assert.NotNull(createdDog);
        }
    }
    
    [Fact]
    public async Task Handle_ShouldCreateDog()
    {
        var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dogToCreate = new Dog { Name = "Dog1", Color = "Brown", TailLength = 20, Weight = 30 };
        var command = new Create.Command { Dog = dogToCreate };
        
        using (var context = new DataContext(dbContextOptions))
        {
            var handler = new Create.Handler(context);
            await handler.Handle(command, CancellationToken.None);
        }
        
        using (var context = new DataContext(dbContextOptions))
        {
            var createdDog = await context.Dogs.FirstOrDefaultAsync(d => d.Name == dogToCreate.Name);
            Assert.NotNull(createdDog);
        }
    }
    
    [Fact]
    public void NameChecker_ShouldReturnTrue_WhenValidName()
    {
        using (var context = new DataContext(_dbContextOptions))
        {
            var dogService = new DogServices(context);
            var validDog = new Dog { Name = "Buddy", Color = "Brown", TailLength = 20, Weight = 30 };
            
            context.Dogs.Add(validDog);
            context.SaveChanges();
            
            Assert.True(dogService.NameChecker(validDog));
        }
    }

    [Fact]
    public void NameChecker_ShouldReturnFalse_WhenInvalidName()
    {
        using (var context = new DataContext(_dbContextOptions))
        {
            var dogService = new DogServices(context);
            var validDog = new Dog { Name = "Buddy", Color = "Brown", TailLength = 20, Weight = 30 };
            var invalidDog = new Dog { Name = "Rex", Color = "Brown", TailLength = 20, Weight = 30 };
            
            context.Dogs.Add(validDog);
            context.SaveChanges();
            
            Assert.False(dogService.NameChecker(invalidDog));
        }
    }

    [Fact]
    public void TailChecker_ShouldReturnTrue_WhenValidTailLength()
    {
        var dogService = new DogServices(null);
        var validDog = new Dog { TailLength = 10 };
        
        var result = dogService.TailChecker(validDog);

        Assert.True(result);
    }

    [Fact]
    public void TailChecker_ShouldReturnFalse_WhenInvalidTailLength()
    {
        var dogService = new DogServices(null);
        var invalidDog = new Dog { TailLength = -5 };

        var result = dogService.TailChecker(invalidDog);
        
        Assert.False(result);
    }
    
    [Fact]
    public void WeightChecker_ShouldReturnTrue_WhenValidTailLength()
    {
        var dogService = new DogServices(null);
        var validDog = new Dog { TailLength = 3 };
        
        var result = dogService.WeightChecker(validDog);

        Assert.True(result);
    }

    [Fact]
    public void WeightChecker_ShouldReturnFalse_WhenInvalidTailLength()
    {
        var dogService = new DogServices(null);
        var invalidDog = new Dog { Weight = -12 };

        var result = dogService.WeightChecker(invalidDog);
        
        Assert.False(result);
    }
}