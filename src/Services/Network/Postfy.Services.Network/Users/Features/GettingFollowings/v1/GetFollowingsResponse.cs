using BuildingBlocks.Core.CQRS.Queries;
using Postfy.Services.Network.Users.Dtos;

namespace Postfy.Services.Network.Users.Features.GettingFollowings.v1;

public record GetFollowingsResponse(ListResultModel<UserBriefDtoWithFollowerCount> Body);
