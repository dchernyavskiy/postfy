using System.Net;
using BuildingBlocks.Core.Exception.Types;

namespace Postfy.Services.Identity.Identity.Exceptions;


public class EmailNotConfirmedException : AppException
{
    public EmailNotConfirmedException(string email)
        : base($"Email not confirmed for email address `{email}`", HttpStatusCode.UnprocessableEntity)
    {
        Email = email;
    }

    public string Email { get; }
}
