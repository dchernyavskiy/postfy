using System.Text.RegularExpressions;
using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Sas;
using Postfy.Services.Network.Medias.Options;
using Postfy.Services.Network.Medias.Services.Contracts;

namespace Postfy.Services.Network.Medias.Services;

public class FileShare : IFileShare
{
    private readonly FileShareOptions _options;
    private readonly ShareServiceClient _shareServiceClient;

    public FileShare(FileShareOptions options)
    {
        _options = options;
        _shareServiceClient = new ShareServiceClient(_options.ConnectionString);
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        var shareClient = _shareServiceClient.GetShareClient(_options.ShareName);
        var extension = Path.GetExtension(file.FileName);
        var directory = shareClient.GetDirectoryClient("files");
        var response = await directory.CreateIfNotExistsAsync();
        var fileClient = directory.GetFileClient(Guid.NewGuid() + extension);

        using (var fs = file.OpenReadStream())
        {
            await fileClient.CreateAsync(fs.Length);
            await fileClient.UploadRangeAsync(new HttpRange(0, fs.Length), fs);
        }

        var sasBuilder = new ShareSasBuilder(ShareFileSasPermissions.Read, DateTimeOffset.MaxValue)
                         {
                             ShareName = _options.ShareName,
                             FilePath = $"files/{fileClient.Name}",
                             Resource = "f",
                             StartsOn = DateTimeOffset.UtcNow,
                         };
        var sas = fileClient.GenerateSasUri(sasBuilder);
        var url = $"{fileClient.Uri.AbsoluteUri}{sas.Query}";

        return url;
    }
}
