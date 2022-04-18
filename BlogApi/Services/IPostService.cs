using BlogApi.Models.PostsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Services
{
    public interface IPostService
    {
        IEnumerable<GetPostResponse> GetPostByUserId(int UserId);
        IEnumerable<GetPostResponse> GetPostByUserIdAndPostId(int UserId, int PostId);
        List<PostResponse> CreatePost(int UserId, PostCreate postCreate);       
        List<PostResponse> UpdatePost(int UserId, PostUpdate postUpdate);
        bool DeletePost(int UserId, PostDelete postDelete);
        bool isTrue(int PostsId, int UserId);

    }
}
