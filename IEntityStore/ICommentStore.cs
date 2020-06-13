using System.Collections.Generic;
using FruityNET.Entities;

namespace FruityNET.IEntityStore
{
    public interface ICommentStore
    {
        List<Comment> GetAllComments();
        Comment AddComment(Comment comment);
        Comment DeleteComment(Comment comment);

        UserAccount GetOwnerOfComment(string UserId);
    }
}