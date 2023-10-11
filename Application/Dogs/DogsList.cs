using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Dogs
{
    public class DogsList
    {
        public class Command : IRequest<List<object>>
        {
            public string AttributeName { get; set; }
            public string OrderType { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
        }

        public class Handler : IRequestHandler<Command, List<object>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<List<object>> Handle(Command request, CancellationToken cancellationToken)
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
    }
}
