using Ardalis.ApiEndpoints;
using Asp.Versioning;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Hellang.Middleware.ProblemDetails;
using Postfy.Services.Network.Medias.Services.Contracts;
using Postfy.Services.Network.Shared.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Postfy.Services.Network.Medias.Features.UploadingMedia.v1;

public record UploadMedia(IFormFileCollection Files) : ICreateCommand<UploadMediaResponse>;

public class UploadMediaHandler : ICommandHandler<UploadMedia, UploadMediaResponse>
{
    private readonly IFileShare _fileShare;

    public UploadMediaHandler(IFileShare fileShare)
    {
        _fileShare = fileShare;
    }

    public async Task<UploadMediaResponse> Handle(UploadMedia request, CancellationToken cancellationToken)
    {
        var medias = new List<Media>();
        foreach (var file in request.Files)
        {
            var url = await _fileShare.UploadAsync(file);
            var media = new Media() {Url = url, Type = file.ContentType};
            medias.Add(media);
        }

        return new UploadMediaResponse(medias);
    }
}

public record UploadMediaResponse(ICollection<Media> Medias);

public class
    UploadMediaEndpoint : EndpointBaseAsync.WithRequest<IFormFileCollection>.WithActionResult<UploadMediaResponse>
{
    private readonly ICommandProcessor _commandProcessor;

    public UploadMediaEndpoint(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpPost(MediaConfigs.PrefixUri, Name = "UploadMedia")]
    [ProducesResponseType(typeof(UploadMediaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(StatusCodeProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "UploadMedia",
        Description = "UploadMedia",
        OperationId = "UploadMedia",
        Tags = new[]
               {
                   MediaConfigs.Tag
               })]
    [ApiVersion(1.0)]
    public override async Task<ActionResult<UploadMediaResponse>> HandleAsync(
        [FromForm] IFormFileCollection files,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        var request = new UploadMedia(files);
        return await _commandProcessor.SendAsync(request, cancellationToken);
    }
}
