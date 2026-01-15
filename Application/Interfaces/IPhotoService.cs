using Application.Profiles.DTOs;

namespace Application.Interfaces;

public interface IPhotoService
{
    Task<PhotoUploadResult?> UploadPhotoAsync(string fileName, Stream fileStream);
    
    Task<string?> DeletePhotoAsync(string publicId);
}
