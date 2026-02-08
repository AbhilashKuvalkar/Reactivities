using Application.Core;
using Application.Interfaces;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries;

public class GetFollowings
{
    public class Query : IRequest<Result<List<UserProfile>>>
    {
        public string Predicate { get; set; } = "followers";

        public required string UserId { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<List<UserProfile>>>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;

        public Handler(AppDbContext appDbContext, IMapper mapper, IUserAccessor userAccessor)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
        }

        public async Task<Result<List<UserProfile>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var profiles = new List<UserProfile>();

            switch (request.Predicate)
            {
                case "followers":
                    profiles = await _appDbContext.UserFollowings
                        .Where(w => string.Equals(w.TargetId, request.UserId))
                        .Select(s => s.Observer)
                        .ProjectTo<UserProfile>(_mapper.ConfigurationProvider, new { currentUserId = _userAccessor.GetUserId() })
                        .ToListAsync(cancellationToken);
                    break;
                case "followings":
                    profiles = await _appDbContext.UserFollowings
                        .Where(w => string.Equals(w.ObserverId, request.UserId))
                        .Select(s => s.Target)
                        .ProjectTo<UserProfile>(_mapper.ConfigurationProvider, new { currentUserId = _userAccessor.GetUserId() })
                        .ToListAsync(cancellationToken);
                    break;
            }

            return Result<List<UserProfile>>.Success(profiles);
        }
    }
}
