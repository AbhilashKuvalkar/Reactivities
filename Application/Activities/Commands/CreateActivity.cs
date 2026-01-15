using Application.Activities.DTOs;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class CreateActivity
{
    public class Command : IRequest<Result<string>>
    {
        public required CreateActivityDto ActivityDto { get; set; }
    }

    public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor) 
        : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userAccessor.GetUserAsync();
            var activity = mapper.Map<CreateActivityDto, Activity>(request.ActivityDto);

            context.Activities.Add(activity);

            var attendee = new ActivityAttendee
            {
                UserId = user.Id,
                ActivityId = activity.Id,
                IsHost = true
            };

            activity.Attendees.Add(attendee);

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            if (!result) return Result<string>.Failure("Failure to create the activity", 404);    

            return Result<string>.Success(activity.Id);
        }
    }
}
