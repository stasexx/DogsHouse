using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.Dogs;

public class Create
{
    public class Command : IRequest
    {
        public Dog Dog { get; set; }
    }
    
    public class Handler : IRequestHandler<Command>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }
        
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _context.Dogs.AddAsync(request.Dog);

            await _context.SaveChangesAsync();
            
        }
    }
}