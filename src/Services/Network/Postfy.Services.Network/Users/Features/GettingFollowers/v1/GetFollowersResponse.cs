using BuildingBlocks.Core.CQRS.Queries;
using Postfy.Services.Network.Users.Dtos;

namespace Postfy.Services.Network.Users.Features.GettingFollowers.v1;

public record GetFollowersResponse(ListResultModel<UserBriefDtoWithFollowerCount> Body);
