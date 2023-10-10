using Domain.Entities;

namespace Application.IServices;

public interface IDogServices
{
    public Task<List<object>> GetDogs();
}