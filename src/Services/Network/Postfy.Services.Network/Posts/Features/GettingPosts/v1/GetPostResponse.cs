using BuildingBlocks.Core.CQRS.Queries;
using Postfy.Services.Network.Posts.Dtos;
using Postfy.Services.Network.Posts.Models;

namespace Postfy.Services.Network.Posts.Features.GettingPost.v1;

public record GetPostResponse(PostBriefDto Body);
