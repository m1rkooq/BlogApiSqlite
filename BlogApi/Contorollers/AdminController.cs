using BlogApi.Models;
using BlogApi.Models.Admins;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
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
    public class AdminController : ControllerBase
    {
        private readonly IAdminServise _service;
        public AdminController(IAdminServise service) =>
            _service = service;

        [AllowAnonymous]
        [HttpGet]
        public IActionResult CreateTable()
        {
            string[] table = {
                "Create table Users(Id INTEGER primary key AUTOINCREMENT, FirstName Nvarchar(30), SecondName Nvarchar(30), Email Nvarchar(50), UserPassword Nvarchar(250), RoleName Nvarchar(50) default('User'))",
                "Create table Posts(Id INTEGER Primary key AUTOINCREMENT, Title Nvarchar(30) not null, Content ntext not null, CreateTime Date not null, UpdateTime Date default 0, UserId INTEGER references Users(Id))",
                "Create table Likes(Id INTEGER primary key AUTOINCREMENT, PostId INTEGER REFERENCES Posts (Id), UserId INTEGER REFERENCES Users (Id))",
                "Create table Comments(Id INTEGER Primary key AUTOINCREMENT, CommText NvarChar(250), CreateTime Date, PostId INTEGER REFERENCES Posts(Id), UserId INTEGER references Users(Id))",
                "Create table Tags(Id INTEGER primary key AUTOINCREMENT, title Nvarchar(250))",
                "Create table PostsTags(TagsId INTEGER references Tags (Id), PostId INTEGER References Posts (Id))"
            };
            using (SqliteConnection conn = new SqliteConnection("DataSource = BlogApiSqlite.db"))
            {

                conn.Open();
                foreach (string tab in table)
                {
                    using (SqliteCommand cmd = new SqliteCommand(tab, conn))
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Table was create");
                    }
                }
                return Ok("Ok!");

            }
        }

        [HttpGet("GetDate")]
        public IActionResult GetPostPerDate([FromBody] AdminsRequestsDate model)
        {
            var query = _service.GetPostPerDate(model);
            if (query != null)
                return Ok(query);
            return BadRequest(new { message = "Not Found"});
        }
        [HttpGet("GetUser")]
        public IActionResult GetPostPerUserId([FromBody] AdminsRequestUser model)
        {
            var query = _service.GetPostPerUser(model.UserId);
            
            if(query != null)
                return Ok(query);
            return BadRequest(new { message = "Not Found" });

        }
        [HttpGet("GetTag")]
        public IActionResult GetPostPerTag([FromBody] AdminsRequestTags model)
        {
            var query = _service.GetPostPerTags(model);
            return Ok(query);
        }        
    }
}
