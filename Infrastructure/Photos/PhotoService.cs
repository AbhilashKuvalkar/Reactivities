using Application.Interfaces;
using Application.Profiles.DTOs;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos;

public class PhotoService : IPhotoService
{
    private readonly ILogger<PhotoService> _logger;
    private readonly Cloudinary _cloudinary;

    public PhotoService(ILogger<PhotoService> logger, IOptions<CloudinarySettings> cloudinaryOptions)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        ArgumentNullException.ThrowIfNull(cloudinaryOptions);
        ArgumentException.ThrowIfNullOrEmpty(nameof(cloudinaryOptions.Value));

        var account = new Account(
            cloudinaryOptions.Value.CloudName,
            cloudinaryOptions.Value.ApiKey,
            cloudinaryOptions.Value.ApiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public async Task<string?> DeletePhotoAsync(string publicId)
    {
        var parameters = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(parameters);

        if (result is not null)
        {
            if (result.Error is null)
            {
                _logger.LogInformation("Photo deleted from Cloudinary successfully. PublicId: {PublicId}", publicId);
                return result.Result;
            }

            _logger.LogError("Error deleting photo from Cloudinary: {ErrorMessage}", result.Error.Message);
            throw new Exception(result.Error.Message);
        }

        return null;
    }

    public async Task<PhotoUploadResult?> UploadPhotoAsync(string fileName, Stream fileStream)
    {
        var parameters = new ImageUploadParams()
        {
            File = new FileDescription(fileName, fileStream),
            Folder = "Reactivities",
            // Transformation = new Transformation().Height(500).Width(500).Crop("fill")
        };

        var result = await _cloudinary.UploadAsync(parameters);

        if (result is not null)
        {
            if (result.Error is null)
            {
                _logger.LogInformation("Photo uploaded to Cloudinary successfully. PublicId: {PublicId}, Url: {Url}", result.PublicId, result.SecureUrl);
                return new PhotoUploadResult
                {
                    PublicId = result.PublicId,
                    Url = result.SecureUrl.AbsoluteUri
                };
            }

            _logger.LogError("Error uploading photo to Cloudinary: {ErrorMessage}", result.Error.Message);
            throw new Exception(result.Error.Message);
        }

        return null;
    }
}
