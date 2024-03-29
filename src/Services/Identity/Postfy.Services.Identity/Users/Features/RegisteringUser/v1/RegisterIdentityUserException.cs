using System.Net;
using BuildingBlocks.Core.Exception.Types;

namespace Postfy.Services.Identity.Users.Features.RegisteringUser.v1;

public class RegisterIdentityUserException : AppException
{
    public RegisterIdentityUserException(string error)
        : base(error, HttpStatusCode.InternalServerError) { }
}
