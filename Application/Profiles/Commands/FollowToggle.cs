using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Persistence;

namespace Application.Profiles.Commands;

public class FollowToggle
{
    public class Command : IRequest<Result<Unit>>
    {
        public required string TargetUserId { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IUserAccessor _userAccessor;

        public Handler(AppDbContext appDbContext, IUserAccessor userAccessor)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var observer = await _userAccessor.GetUserAsync();
            var target = await _appDbContext.Users
                .FindAsync([request.TargetUserId], cancellationToken);

            if (target is null)
                return Result<Unit>.Failure("Target user not found", 400);

            var following = await _appDbContext.UserFollowings
                .FindAsync([observer.Id, target.Id], cancellationToken);

            if (following is null)
            {
                await _appDbContext.UserFollowings
                    .AddAsync(new UserFollowing()
                    {
                        ObserverId = observer.Id,
                        TargetId = target.Id
                    }, cancellationToken);
            }
            else 
                _appDbContext.UserFollowings.Remove(following);

            return await _appDbContext.SaveChangesAsync(cancellationToken) > 0 
                ? Result<Unit>.Success(Unit.Value) 
                : Result<Unit>.Failure("Problem updating following", 400);
        }
    }
}
