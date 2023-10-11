using Domain.Entities;

namespace Persistence;

public class DataSeed
{
    public static async Task SeedData(DataContext contex)
    {
        if (contex.Dogs.Any()) return;
        
        var dogs = new List<Dog>()
        {
            new Dog()
            {
                Name = "Neo",
                Color = "red&amber",
                TailLength = 22,
                Weight = 32
            },

            new Dog()
            {
                Name = "Jessy",
                Color = "black&white",
                TailLength = 7,
                Weight = 14
            }
        };
        
        await contex.Dogs.AddRangeAsync(dogs);
        await contex.SaveChangesAsync();
    }
}