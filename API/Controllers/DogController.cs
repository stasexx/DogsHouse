using Application.Dogs;
using Application.IServices;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class DogController : BaseController
{
    private readonly IDogServices _dogServices;
    
    public DogController(IDogServices dogServices)
    {
        _dogServices = dogServices;
    }
    
    [HttpGet("dogs")]
    public async Task<IActionResult> GetDogs(string attribute = "name", string order = "asc", int pageNumber = 1, int pageSize = 10)
    {
        var dogs = await Mediator.Send(new DogList.Command
        {
            AttributeName = attribute, 
            OrderType = order,
            PageNumber = pageNumber,
            PageSize = pageSize
        });

        return Ok(dogs);
    }
    
    [HttpPost("dog")]
    public async Task<IActionResult> CreateDog([FromBody]Dog dog)
    {
        if (dog == null)
        {
            return BadRequest("Invalid JSON data in the request body.");
        }
        
        if (_dogServices.NameChecker(dog))
        {
            return Conflict("A dog with the same name already exists.");
        }
        
        if (!_dogServices.TailChecker(dog) )
        {
            return BadRequest("Tail length cannot be negative.");
        }
        
        if (!_dogServices.WeightChecker(dog) )
        {
            return BadRequest("Weight cannot be negative and zero.");
        }
        
        dog.Id = Guid.NewGuid();
        
        await Mediator.Send(new Create.Command {Dog = dog});

        return Ok();
    }
}