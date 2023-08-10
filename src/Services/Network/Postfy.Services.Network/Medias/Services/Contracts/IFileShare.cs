namespace Postfy.Services.Network.Medias.Services.Contracts;

public interface IFileShare
{
    Task<string> UploadAsync(IFormFile file);
}
