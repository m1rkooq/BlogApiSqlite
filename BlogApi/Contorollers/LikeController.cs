using BlogApi.Models;
using BlogApi.Models.LikesModels;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Contorollers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LikeController: ControllerBase
    {
        private readonly ILikeService _service;
        private int _userId;
        public LikeController(ILikeService service, IHttpContextAccessor httpContextAccessor) 
        {
            _service = service;
            _userId = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString());
        }

            

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetCountLikesByPostId([FromBody] Likes model)
        {
            var query = _service.GetCountLikeByPostId(model);
            return Ok($"Count like on postId: {model.PostId} - {query}");            
        }

        [HttpPost]
        public IActionResult LikePost([FromBody] Likes model)
        {   
            if(_userId != 0)
            {
                var like = _service.LikeCheck(_userId, model);

                if (like == true)
                    return Ok($"UnLike PostId: {model.PostId}, UserId: {_userId}");
                else
                    return Ok($"Like PostId: {model.PostId}, UserId: {_userId}");
            }
            return BadRequest(new { messsage = "Error!" });
            
        }

        /*private UserResponce GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new UserResponce
                {
                    Id = Convert.ToInt32(userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value),
                    FirstName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Surname)?.Value,                    
                    Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value
                };
            }
            return null;
        }*/

    }
}
