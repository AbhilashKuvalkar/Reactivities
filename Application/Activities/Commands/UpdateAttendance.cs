using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Commands;

public class UpdateAttendance
{
    public class Command : IRequest<Result<Unit>>
    {
        public required string Id { get; set; }
    }

    public class Handler(AppDbContext context, IUserAccessor userAccessor) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities
                .Include(a => a.Attendees)
                .ThenInclude(ua => ua.User)
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (activity is null) return Result<Unit>.Failure("Activity not found", 404);

            var user = await userAccessor.GetUserAsync();
            var attendance = activity.Attendees.FirstOrDefault(a => a.User.Id == user.Id);
            var isHost = activity.Attendees.Any(u => u.IsHost && string.Equals(u.User.Id, user.Id, StringComparison.OrdinalIgnoreCase));

            if (attendance is not null)
            {
                if (isHost)
                    activity.IsCancelled = !activity.IsCancelled;
                else
                    activity.Attendees.Remove(attendance);
            }
            else
            {
                activity.Attendees.Add(new Domain.ActivityAttendee
                {
                    UserId = user.Id,
                    ActivityId = activity.Id,
                    IsHost = false
                });
            }

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            return result
                ? Result<Unit>.Success(Unit.Value)
                : Result<Unit>.Failure("Failed to update attendance", 400);
        }
    }

}
