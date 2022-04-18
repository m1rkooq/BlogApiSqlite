using BlogApi.Models.LikesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Services
{
    public interface ILikeService
    {
        bool LikeCheck(int UserId, Likes model);
        void UnLikePost(int UserId, Likes model);
        void LikePost(int UserId, Likes model);
        int GetCountLikeByPostId(Likes model);
    }
}
