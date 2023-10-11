using System.Linq.Expressions;
using Application.Models;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Dogs
{
    public class DogList
    {
        public class Command : IRequest<List<DogDto>>
        {
            public string AttributeName { get; set; }
            public string OrderType { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
        }

        public class Handler : IRequestHandler<Command, List<DogDto>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<List<DogDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var orderBy = request.AttributeName switch
                {
                    "name" => (Expression<Func<Dog, object>>)(dog => dog.Name),
                    "color" => (Expression<Func<Dog, object>>)(dog => dog.Color),
                    "tail_length" => (Expression<Func<Dog, object>>)(dog => dog.TailLength),
                    "weight" => (Expression<Func<Dog, object>>)(dog => dog.Weight),
                    _ => (Expression<Func<Dog, object>>)(dog => dog.Name)
                };

                var orderedDogs = request.OrderType.ToLower() == "desc"
                    ? await _context.Dogs.OrderByDescending(orderBy).ToListAsync()
                    : await _context.Dogs.OrderBy(orderBy).ToListAsync();

                int skip = (request.PageNumber - 1) * request.PageSize;
                var pagedDogs = orderedDogs.Skip(skip).Take(request.PageSize);

                return pagedDogs.Select(dog => new DogDto
                {
                    Name = dog.Name,
                    Color = dog.Color,
                    TailLength = dog.TailLength,
                    Weight = dog.Weight
                }).ToList();
            }
        }
    }
}
