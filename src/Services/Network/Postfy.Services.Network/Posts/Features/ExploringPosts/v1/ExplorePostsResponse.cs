using BuildingBlocks.Core.CQRS.Queries;
using Postfy.Services.Network.Posts.Dtos;

namespace Postfy.Services.Network.Posts.Features.ExploringPosts.v1;

public record ExplorePostsResponse(ListResultModel<PostBriefDto> Body, Guid LastPostId);
