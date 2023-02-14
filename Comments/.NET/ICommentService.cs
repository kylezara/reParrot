using Sabio.Models;
using Sabio.Models.Domain.Comments;
using Sabio.Models.Requests.Comments;
using System.Collections.Generic;

namespace Sabio.Services.Interfaces
{
    public interface ICommentService
    {
        List<Comment> GetByEntityId(int entityId, int entityTypeId);
        List<Comment> GetNestedComments(int entityId, int entityTypeId);
        int Add(CommentAddRequest model, int currentUserId);
        void Update(CommentUpdateRequest model, int currentUserId);
        void Delete(int id); 
        Paged<Comment> CommentsGetPaged(int pageIndex, int pageSize); 
        List<Comment> GetByCreatedBy(int createdBy);

    }
}
