namespace Domain.Entities;

public class Dog
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Color { get; set; }
    
    public double TailLength { get; set; }
    
    public double Weight { get; set; }
}