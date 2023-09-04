using BuildingBlocks.Core.CQRS.Queries;
using Postfy.Services.Network.Users.Dtos;

namespace Postfy.Services.Network.Users.Features.GettingSuggestions.v1;

public record GetSuggestionsResponse(ListResultModel<UserBriefDtoWithFollowerCount> Body);
