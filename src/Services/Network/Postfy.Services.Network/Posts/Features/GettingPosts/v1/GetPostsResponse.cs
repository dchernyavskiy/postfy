using BuildingBlocks.Core.CQRS.Queries;
using Postfy.Services.Network.Posts.Dtos;
using Postfy.Services.Network.Posts.Models;

namespace Postfy.Services.Network.Posts.Features.GettingPosts.v1;

public record GetPostsResponse(ListResultModel<PostBriefDto> Body);
