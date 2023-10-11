using Domain.Entities;

namespace Application.IServices;

public interface IDogServices
{
    public bool NameChecker(Dog dog);
    
    public bool TailChecker(Dog dog);
}