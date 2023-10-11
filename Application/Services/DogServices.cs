using Application.IServices;
using Domain.Entities;
using Persistence;

namespace Application.Services;

public class DogServices : IDogServices
{
    private readonly DataContext _context;

    public DogServices(DataContext context)
    {
        _context = context;
    }
    
    public bool NameChecker(Dog dog) => _context.Dogs.Any(d => d.Name == dog.Name);

    public bool TailChecker(Dog dog)
    {
        if (dog.TailLength < 0)
        {
            return false;
        }
        return true;
    }
    
    public bool WeightChecker(Dog dog)
    {
        if (dog.Weight <= 0)
        {
            return false;
        }
        return true;
    }
}