using BuildingBlocks.Core.CQRS.Queries;
using Postfy.Services.Network.Posts.Dtos;
using Postfy.Services.Network.Posts.Models;

namespace Postfy.Services.Network.Posts.Features.GettingSavedPosts.v1;

public record GetSavedPostsResponse(ListResultModel<PostBriefDto> Body);
