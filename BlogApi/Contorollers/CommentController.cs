using BlogApi.Helper;
using BlogApi.Models;
using BlogApi.Models.CommentsModels;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _service;
        private int _userId;
        public CommentController(ICommentService service, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _userId = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString());
        }

        [AllowAnonymous]
        [HttpGet("blogs/{UserId}/posts/{PostId}/Comments")]
        public IActionResult GetCommentPyPostId(int PostId)
        {
            var query = _service.GetCommentsByPostId(PostId);
            if(query != null)
            {
                return Ok(query);
            }
            return Ok($"No comments PostId: {PostId}");
        }

            

        [HttpPost]
        public IActionResult CreateComment([FromBody]CommentCreate comment)
        {
            
            var command = _service.CommentCreate(_userId, comment);
            if(command != null)
            {
                return Ok(command);
            }
            return BadRequest(new { message = "Error!" });
            
        }


        [HttpDelete]
        public IActionResult DeleteComment([FromBody]CommentDelete comment)
        {            
            var check = _service.isTrue(_userId, comment);

            if (check != false)
            {
                var command = _service.CommentDelete(_userId, comment);
                if(command != 0)
                    return Ok($"Delete success UserID: {_userId}, PostId: {comment.PostId}, CommentId: {comment.Id}");
            }
            return Ok("You Can't delete comment!");
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
