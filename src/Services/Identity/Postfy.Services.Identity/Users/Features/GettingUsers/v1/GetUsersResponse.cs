using BuildingBlocks.Core.CQRS.Queries;
using Postfy.Services.Identity.Users.Dtos;
using Postfy.Services.Identity.Users.Dtos.v1;

namespace Postfy.Services.Identity.Users.Features.GettingUsers.v1;

public record GetUsersResponse(ListResultModel<IdentityUserDto> IdentityUsers);
