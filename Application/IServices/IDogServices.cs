using Domain.Entities;

namespace Application.IServices;

public interface IDogServices
{
    public Task<List<object>> GetDogs(string attribute, string order, int pageNumber, int pageSize);
}