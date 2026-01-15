using Application.Core;
using Application.Interfaces;
using MediatR;
using Persistence;

namespace Application.Profiles.Commands;

public class DeletePhoto
{
    public class Command : IRequest<Result<Unit>>
    {
        public required string PhotoId { get; set; }
    }

    public class Handler(AppDbContext appDbContext, IUserAccessor userAccessor, IPhotoService photoService)
        : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userAccessor.GetUserWithPhotosAsync();
            var photo = user.Photos.FirstOrDefault(x => x.Id.Equals(request.PhotoId));

            if (photo is null)
                return Result<Unit>.Failure("Photo not found.", 400);

            if (photo.Url.Equals(user.ImageUrl))
                return Result<Unit>.Failure("Cannot delete main photo.", 400);

            await photoService.DeletePhotoAsync(photo.PublicId);
            user.Photos.Remove(photo);

            var result = await appDbContext.SaveChangesAsync(cancellationToken) > 0;
            return result
                ? Result<Unit>.Success(Unit.Value)
                : Result<Unit>.Failure("Failed to delete photo.", 400);
        }
    }
}
