using System.Linq.Expressions;
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

    public async Task<List<object>> GetDogs(string attribute, string order, int pageNumber, int pageSize)
    {
        var orderBy = attribute switch
        {
            "name" => (Expression<Func<Dog, object>>)(dog => dog.Name),
            "color" => dog => dog.Color,
            "tail_length" => dog => dog.TailLength,
            "weight" => dog => dog.Weight,
            _ => dog => dog.Name
        };

        var orderedDogs = order.ToLower() == "desc"
            ? await _context.Dogs.OrderByDescending(orderBy).ToListAsync()
            : await _context.Dogs.OrderBy(orderBy).ToListAsync();

        int skip = (pageNumber - 1) * pageSize;
        var pagedDogs = orderedDogs.Skip(skip).Take(pageSize);

        var result = pagedDogs.Select(dog => (object)new
        {
            dog.Name,
            dog.Color,
            dog.TailLength,
            dog.Weight
        }).ToList();

        return result;
    }
}