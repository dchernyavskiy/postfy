using BuildingBlocks.Core.CQRS.Queries;
using Postfy.Services.Network.Posts.Dtos;

namespace Postfy.Services.Network.Posts.Features.GettingFeed.v1;

public record GetFeedResponse(ListResultModel<PostBriefDto> Body);
