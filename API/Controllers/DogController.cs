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
    public async Task<List<object>> GetDogs()
    {
        return await _dogServices.GetDogs();
    }
}