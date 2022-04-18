using BlogApi.Models.CommentsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Services
{
    public interface ICommentService
    {
        List<CommentResponse> CommentCreate(int UserId, CommentCreate commentCreate);
        int CommentDelete(int UserId, CommentDelete commentDelete);
        bool isTrue(int UserId, CommentDelete commentDelete);
        IEnumerable<CommentResponse> GetCommentsByPostId(int PostId);

    }
}
