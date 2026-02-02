using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries;

public class GetProfilePhotos
{
    public class Query : IRequest<Result<List<Photo>>>
    {
        public required string UserId { get; set; }
    }

    public class Handler(AppDbContext appDbContext, IUserAccessor userAccessor)
        : IRequestHandler<Query, Result<List<Photo>>>
    {
        public async Task<Result<List<Photo>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var currentUser = userAccessor.GetUserId();

            var photos = await appDbContext.Users
                .Where(x => x.Id.Equals(request.UserId))
                .SelectMany(x => x.Photos)
                .ToListAsync(cancellationToken);

            return Result<List<Photo>>.Success(photos);
        }
    }
}
