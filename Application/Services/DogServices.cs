using Application.IServices;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Services;

public class DogServices : IDogServices
{
    private readonly DataContext _context;

    public DogServices(DataContext context)
    {
        _context = context;
    }

    public async Task<List<object>> GetDogs()
    {
        var dogs = await _context.Dogs
            .Select(dog => new
            {
                Name = dog.Name,
                Color = dog.Color,
                TailLength = dog.TailLength,
                Weight = dog.Weight
            })
            .ToListAsync();

        return dogs.Cast<object>().ToList();
    }

}