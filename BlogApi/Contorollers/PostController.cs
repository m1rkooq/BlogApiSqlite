using BlogApi.Models;
using BlogApi.Models.PostsModels;
using BlogApi.Models.TagsModels;
using BlogApi.Services;
using BlogApiSqlite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Contorollers
{
    //[Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostService _service;
        private readonly ITagService _tag;
        private int _userId;

        public PostController(IPostService service, ITagService tag, IHttpContextAccessor httpContextAccessor)
        { 
            _service = service;
            _tag = tag;
            _userId = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString());
        }

        [AllowAnonymous]
        [HttpGet("blogs/{UserId}/posts/")] //все посты юзера
        public IActionResult GetPostsByUserId(int UserId)
        {
            var query = _service.GetPostByUserId(UserId);
            if(query != null)
            {                
                return Ok(query);
            }
            return BadRequest(new { message = "Not found!"});
        }

        [AllowAnonymous]
        [HttpGet("blogs/{UserId}/posts/{PostId}")] // пост юзера по постИд
        public IActionResult GetPistsByUserIdAndPostId(int UserId, int PostId)
        {
            var query = _service.GetPostByUserIdAndPostId(UserId, PostId);
            if (query != null)
            {
                return Ok(query);
            }
            return BadRequest(new { message = "Not found!" });
        }


        [HttpPost]
        public IActionResult CreatePost([FromBody] PostCreate post)
        {           
            var command = _service.CreatePost(_userId, post);
            if (command != null)
            {
                return Ok(command);
            }
            return BadRequest(new { message = "Post don't create!" });            
        }
       

        [HttpPut]
        public IActionResult UpdatePost([FromBody] PostUpdate post)
        {           
            var check = _service.isTrue(post.Id, _userId);
            if(check != false)
            {
                var command = _service.UpdatePost(_userId, post);

                if (command != null)
                {
                    return Ok(command);
                }
                
            }

            return BadRequest(new { message = "U can't edit that post!" });
        }

        [HttpPut("Tag")]
        public IActionResult UpdateTag([FromBody] Tags model)
        {
            var check = _tag.isTrue(_userId, model);
            if(check == true)
                return BadRequest(new { message = "U can't edit tag!" });

            var command = _tag.UpdateTag(model);

            if(command != null)
            {
                return Ok(command);
            }
            return BadRequest(new { message = "Tag don't edite!" });
        }


        [HttpDelete]
        public IActionResult DeletePost([FromBody] PostDelete post)
        {            
            var check =  _service.isTrue(post.Id, _userId);

            if (check != false)
            {
                var command = _service.DeletePost(_userId, post);

                if(command != false)
                {
                    return Ok($"Post was delete PostId: {post.Id}. UserId: {_userId}");
                }
                return NoContent();
            }

            return BadRequest(new { message = "U can't delete that post!" });
        }

        [HttpDelete("Tag")]
        public IActionResult DeleteTags([FromBody] Tags model)
        {
            var check = _tag.isTrue(_userId, model);
            if(check == true)
                return BadRequest(new { message = "U can't delete that tag!" });

            var command = _tag.DeleteTag(model);
            if(command != false)
            {
                Ok($"Delete Tags {model.Id}");
            }
            return NoContent();
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
