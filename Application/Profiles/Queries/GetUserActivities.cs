using Application.Core;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries;

public class GetUserActivities
{
    public class Query : IRequest<Result<List<UserActivityDto>>>
    {
        public required string UserId { get; set; }

        public required string Filter { get; set; }
    }

    public class Handler(AppDbContext appDbContext, IMapper mapper) :
        IRequestHandler<Query, Result<List<UserActivityDto>>>
    {
        public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = appDbContext.ActivityAttendees
                .Where(w => w.User.Id == request.UserId)
                .OrderBy(o => o.Activity.Date)
                .Select(s => s.Activity)
                .AsQueryable();

            var today = DateTime.UtcNow;

            query = request.Filter switch
            {
                "past" => query.Where(w => w.Date <= today && w.Attendees.Any(a => a.UserId == request.UserId)),
                "hosting" => query.Where(w => w.Attendees.Any(a => a.IsHost && a.UserId == request.UserId)),
                _ => query.Where(w => w.Date >= today && w.Attendees.Any(a => a.UserId == request.UserId))
            };

            var projectedActivities = query.ProjectTo<UserActivityDto>(mapper.ConfigurationProvider);

            var activities = await projectedActivities.ToListAsync(cancellationToken);

            return Result<List<UserActivityDto>>.Success(activities);
        }
    }
}
