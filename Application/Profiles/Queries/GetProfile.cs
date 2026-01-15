using Application.Core;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries;

public class GetProfile
{
    public class Query : IRequest<Result<UserProfile>>
    {
        public required string UserId { get; set; }
    }

    public class Handler(AppDbContext appDbContext, IMapper mapper)
        : IRequestHandler<Query, Result<UserProfile>>
    {
        public async Task<Result<UserProfile>> Handle(Query request, CancellationToken cancellationToken)
        {
            var profile = await appDbContext.Users
                .ProjectTo<UserProfile>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id.Equals(request.UserId), cancellationToken);

            return profile is null 
                ? Result<UserProfile>.Failure("Profile not found", 404)
                : Result<UserProfile>.Success(profile);
        }
    }
}
