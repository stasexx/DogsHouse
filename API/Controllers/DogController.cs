using Application.IServices;
using Domain.Entities;
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
        var dogs = await _dogServices.GetDogs(attribute, order, pageNumber, pageSize);

        return Ok(dogs);
    }
}