using System.Net;
using BuildingBlocks.Core.Exception.Types;

namespace Postfy.Services.Network.Posts.Exceptions.Application;

public class PostNotFoundException : AppException
{
    public PostNotFoundException(Guid id)
        : base($"Post with id '{id}' not found", HttpStatusCode.NotFound)
    {
    }

    public PostNotFoundException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(
        message,
        statusCode)
    {
    }
}
