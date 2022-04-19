using BlogApi.Models.PostsModels;
using BlogApi.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BlogApi.Models.TagsModels;
using BlogApi.Models;
using Microsoft.Data.Sqlite;

namespace BlogApi.DataLayer
{
    
    public class PostService : IPostService
    {
        private string _config;

        public PostService(IConfiguration configuration)
        {
            _config = configuration.GetConnectionString("DefaultConnection");
        }


        public List<PostResponse> CreatePost(int UserId, PostCreate postCreate)
        {           
            List<PostResponse> postList = new List<PostResponse>();
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                
                string CommandString = "INSERT INTO Posts(Title, Content, CreateTime, UpdateTime, UserId) " +
                    "VALUES (@Title, @Content, @CreateTime, @UpdateTime, @UserId);" +
                    "SELECT last_insert_rowid();";
                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", postCreate.Title);
                    cmd.Parameters.AddWithValue("@Content", postCreate.Content);
                    cmd.Parameters.AddWithValue("@CreateTime", postCreate.CreatedDate);
                    cmd.Parameters.AddWithValue("@UpdateTime", postCreate.UpdatedDate);
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    /*cmd.Parameters.Add("@ReturnId", SqliteType.Integer, 4);
                    cmd.Parameters["@ReturnId"].Direction = ParameterDirection.Output;*/

                    try
                    {
                        
                        var Id = cmd.ExecuteScalar();
                        var PostId = Int32.Parse(Id.ToString());

                        if(PostId != 0)
                        {
                            var tags = CreateTags(PostId, postCreate.tags);
                            var user = GetUserInfo(UserId);
                            postList.Add(new PostResponse
                            {
                                User = user,
                                Id = PostId,
                                Title = postCreate.Title,
                                Content = postCreate.Content,
                                CreatedDate = postCreate.CreatedDate,
                                UpdateDate = postCreate.UpdatedDate,
                                Tags = tags
                            });
                        }
          
                        return postList;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return null;
        }
        private void disFK()
        {
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string CommandString = "PRAGMA foreign_keys = OFF;";
                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {                  
                    try
                    {
                        cmd.ExecuteNonQuery();                        
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
        private void actFK()
        {
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string CommandString = "PRAGMA foreign_keys = ON;";
                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }


        public List<PostResponse> UpdatePost(int UserId, PostUpdate postUpdate)
        {
            List<PostResponse> postList = new List<PostResponse>();
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string CommandString = "UPDATE Posts SET " +
                    "Title = @Title, " +
                    "Content = @Content, " +
                    "UpdateTime = @UpdateTime " +
                    "WHERE Id = @Id And " +
                    "UserId = @UserId";

                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", postUpdate.Id);
                    cmd.Parameters.AddWithValue("@Title", postUpdate.Title);
                    cmd.Parameters.AddWithValue("@Content", postUpdate.Content);
                    cmd.Parameters.AddWithValue("@UpdateTime", postUpdate.UpdateDate);
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    try
                    {
                        
                        cmd.ExecuteNonQuery();

                        var user = GetUserInfo(UserId);
                        postList.Add(new PostResponse
                        {
                            User = user,
                            Id = postUpdate.Id,
                            Content = postUpdate.Content,                            
                            UpdateDate = postUpdate.UpdateDate,
                            Tags = GetTagList(postUpdate.Id)
                        });
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
            return postList;
        }

        public bool DeletePost(int UserId, PostDelete postDelete)
        {            
            bool Return = false;            
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                disFK();
                string CommandString = "PRAGMA foreign_keys = OFF; DELETE FROM Posts WHERE Id = @Id AND UserId = @UserId;";
                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", postDelete.Id);
                    cmd.Parameters.AddWithValue("@UserId", UserId);                  

                    try
                    {                        
                        cmd.ExecuteNonQuery();
                        Return = true;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            actFK();
            return Return;
        }



        public IEnumerable<GetPostResponse> GetPostByUserId(int UserId)
        {            
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string QueryString = "SELECT u.Id, u.FirstName, u.SecondName, p.Id AS PostId ,p.Title , p.Content ,p.CreateTime, p.UpdateTime, " +
                    "(SELECT Count(*) FROM Comments WHERE Comments.PostId = PostId) AS CommentCount, (SELECT Count(*) FROM Likes WHERE PostId = PostId) AS CountLike FROM Users AS u, Posts AS p " +
                    "WHERE p.UserId = @UserId";
                using (SqliteCommand cmd = new SqliteCommand(QueryString, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);                    
                    
                    var reader = cmd.ExecuteReader();                    
                    while (reader.Read())
                    {                        
                        var post = new GetPostResponse();

                        post.user = new List<UserResponce>();

                        post.user.Add(new UserResponce() {
                            Id = UserId,
                            FirstName = reader["FirstName"].ToString(),
                            SecondName = reader["SecondName"].ToString()
                        });
                        post.Id = Convert.ToInt32(reader["PostId"].ToString());
                        post.Title = reader["Title"].ToString();
                        post.Content = reader["Content"].ToString();
                        post.CreatedDate = Convert.ToDateTime(reader["CreateTime"].ToString()).ToString("f");
                        post.UpdateDate = Convert.ToDateTime(reader["UpdateTime"].ToString()).ToString("f");
                        post.CountComments = Convert.ToInt32(reader["CommentCount"].ToString());
                        post.CountLike = Convert.ToInt32(reader["CountLike"].ToString());
                        post.TagList = GetTagList(post.Id);

                        yield return post;
                    }
                }
            }
        }

        public IEnumerable<GetPostResponse> GetPostByUserIdAndPostId(int UserId, int PostId)
        {
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string QueryString = "Select u.Id, u.FirstName, u.SecondName, p.Id as PostId ,p.Title , p.Content ,p.CreateTime, p.UpdateTime, " +
                    "(Select Count(*) from Comments where Comments.PostId = @PostId) as CommentCount, (Select Count(*) from Likes where PostId = @PostId) as CountLike from Users as u, Posts as p " +
                    "Where p.UserId = @UserId and p.Id = @PostId";
                using (SqliteCommand cmd = new SqliteCommand(QueryString, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@PostId", PostId);

                                        
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var post = new GetPostResponse();
                        post.user = new List<UserResponce>();
                        post.user.Add(new UserResponce()
                        {
                            Id = UserId,
                            FirstName = reader["FirstName"].ToString(),
                            SecondName = reader["SecondName"].ToString()
                        });
                        post.Id = Convert.ToInt32(reader["PostId"].ToString());
                        post.Title = reader["Title"].ToString();
                        post.Content = reader["Content"].ToString();
                        post.CreatedDate = Convert.ToDateTime(reader["CreateTime"].ToString()).ToString("f");
                        post.UpdateDate = Convert.ToDateTime(reader["UpdateTime"].ToString()).ToString("f");
                        post.CountComments = Convert.ToInt32(reader["CommentCount"].ToString());
                        post.CountLike = Convert.ToInt32(reader["CountLike"].ToString());
                        post.TagList = GetTagList(PostId);

                        yield return post;
                    }
                }
            }
        }

        public bool isTrue(int PostId, int UserId)
        {
            bool res = false;
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                using (SqliteCommand cmd = new SqliteCommand("Select Count(*) from Posts Where Id = @Id and UserId = @UserId", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", PostId);
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    try
                    {
                        
                        var Count = cmd.ExecuteScalar();
                        
                        if (Convert.ToInt32(Count.ToString()) > 0)
                            res = true;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return res;
        }


        private List<Tags> CreateTags(int PostId, string[] tags)
        {
            List<Tags> tagList = new List<Tags>();
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                foreach (string tag in tags)
                {
                    string CommandString = "INSERT INTO Tags(title) VALUES (@Title);" +
                        "SELECT last_insert_rowid();";
                    using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", tag);
                        

                        try
                        {
                            var Id = cmd.ExecuteScalar();                             

                            tagList.Add(new Tags()
                            {
                                Id = Convert.ToInt32(Id.ToString()),
                                TagName = tag
                            });
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                AddInPostTags(PostId, tagList);

            }
            return tagList;
        }


        private void AddInPostTags(int PostId, List<Tags> tagList)
        {
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                
                foreach (Tags tag in tagList)
                {
                    string CommandString = "INSERT INTO PostsTags(TagsId, PostId) VALUES (@TagsId, @PostId);";
                    using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TagsId", tag.Id);
                        cmd.Parameters.AddWithValue("@PostId", PostId);

                        try
                        {                            
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }

        private List<Tags> GetTagList(int PostId)
        {
            List<Tags> tags = new List<Tags>();
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string CommandString = " SELECT Tags.Id AS Id, Tags.title AS title FROM PostsTags " +
                    "JOIN Tags ON PostsTags.TagsId = Tags.Id AND PostsTags.PostId = @PostId";
                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@PostId", PostId);

                    try
                    {                        
                        var reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            tags.Add(new Tags()
                            {
                                Id = Convert.ToInt32(reader["Id"].ToString()),
                                TagName = reader["title"].ToString()                               
                            });                            
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }
                return tags;
            }
            
        }
        private List<UserResponce> GetUserInfo(int UserId)
        {
            List<UserResponce> tags = new List<UserResponce>();
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string CommandString = " SELECT Id, FirstName, SecondName FROM Users WHERE Id = @UserId";
                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    try
                    {                        
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            tags.Add(new UserResponce()
                            {
                                Id = Convert.ToInt32(reader["Id"].ToString()),
                                FirstName = reader["FirstName"].ToString(),
                                SecondName = reader["SecondName"].ToString(),
                            });
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return tags;
        }

        /*private void DeleteCommByPost(int PostId)
        {
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string CommandString = "DELETE FROM Comments WHERE PostId = @PostId;";
                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@PostId", PostId);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }*/
    }
}
