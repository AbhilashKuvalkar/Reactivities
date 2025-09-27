using System;
using Application.Exceptions;
using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class DeleteActivity
{
    public class Command : IRequest
    {
        public required string Id { get; set; }
    }

    class Handler(AppDbContext context) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities
                .FindAsync([request.Id], cancellationToken) ??
                    throw new NotFoundException($"Activity with ID '{request.Id}' not found.");

            context.Activities.Remove(activity);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
