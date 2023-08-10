using Postfy.Services.Network.Comments.Models;

namespace Postfy.Services.Network.Comments.Features.GettingCommentsByPostId.v1;

public record GetCommentsByPostIdResponse(List<Comment> Comments);
