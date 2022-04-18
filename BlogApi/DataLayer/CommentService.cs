using BlogApi.Models;
using BlogApi.Models.CommentsModels;
using BlogApi.Services;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.DataLayer
{
    public class CommentService : ICommentService
    {
        private string _config;

        public CommentService(IConfiguration configuration) =>
            _config = configuration.GetConnectionString("DefaultConnection");

        public List<CommentResponse> CommentCreate(int UserId, CommentCreate commentCreate)
        {
            List<CommentResponse> commentResponseList = new List<CommentResponse>();
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string CommandString = "INSERT INTO Comments(CommText, CreateTime, PostId, UserId) " +
                    "VALUES(@CommText, @CreateTime, @PostId, @UserId);" +
                    "SELECT last_insert_rowid();";
                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@CommText", commentCreate.CommentText);
                    cmd.Parameters.AddWithValue("@CreateTime", commentCreate.CreateTime);
                    cmd.Parameters.AddWithValue("@PostId", commentCreate.PostId);
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    

                    try
                    {
                                             
                        var Id = cmd.ExecuteScalar();

                        commentResponseList.Add(new CommentResponse()
                        {
                            Users = GetUserInfo(UserId),
                            CommentId = Convert.ToInt32(Id.ToString()),
                            CommentText = commentCreate.CommentText,
                            CreateTime = commentCreate.CreateTime,
                            PostId = commentCreate.PostId
                        });
                        return commentResponseList;
                    }
                    catch(Exception ex)
                    {

                    }


                }
            }
            return null;
        }

        public int CommentDelete(int UserId, CommentDelete commentDelete)
        {
            int Result = 0;
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string CommandString = "DELETE FROM Comments WHERE @UserId = UserId AND " +
                    "@PostId = PostId AND @Id = Id;";
                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", commentDelete.Id);
                    cmd.Parameters.AddWithValue("@PostId", commentDelete.PostId);
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    try
                    {
                        
                        cmd.ExecuteNonQuery();                        
                        Result = 1;
                    }
                    catch(Exception ex)
                    {

                    }
                }

            }
            return Result;

        }

        public bool isTrue(int UserId, CommentDelete commentDelete)
        {
            bool Result = false;
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                using (SqliteCommand cmd = new SqliteCommand("SELECT Count(*) FROM Comments WHERE Id = @Id AND UserId = @UserId AND PostId = @PostId;", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", commentDelete.Id);
                    cmd.Parameters.AddWithValue("@PostId", commentDelete.PostId);
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    try
                    {
                       
                        var Count = cmd.ExecuteScalar();

                        if (Convert.ToInt32(Count.ToString()) > 0)
                            Result = true;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return Result;
        }

        public IEnumerable<CommentResponse> GetCommentsByPostId(int PostId)
        {
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string QueryString = "SELECT u.Id AS UserId, u.FirstName, u.SecondName, c.Id AS ComId, c.CommText, c.CreateTime " +
                    "FROM Users AS u, Comments AS c WHERE c.PostId = @PostId;";
                using (SqliteCommand cmd = new SqliteCommand(QueryString, conn))
                {
                    cmd.Parameters.AddWithValue("@PostId", PostId);

                                     
                    var reader = cmd.ExecuteReader();                   
                    while (reader.Read())
                    {
                        var postComment = new CommentResponse();
                        postComment.Users = new List<UserResponce>();
                        postComment.Users.Add(new UserResponce()
                        {
                            Id = Convert.ToInt32(reader["UserId"].ToString()),
                            FirstName = reader["FirstName"].ToString(),
                            SecondName = reader["SecondName"].ToString()
                        });
                        postComment.CommentId = Convert.ToInt32(reader["ComId"].ToString());
                        postComment.CommentText = reader["CommText"].ToString();
                        postComment.CreateTime = Convert.ToDateTime(reader["CreateTime"].ToString()).ToString("f");
                        postComment.PostId = PostId;

                        yield return postComment;
                    }
                }

            }
        }


        private List<UserResponce> GetUserInfo(int UserId)
        {
            List<UserResponce> users = new List<UserResponce>();
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string CommandString = "SELECT FirstName, SecondName FROM Users WHERE Id = @UserId;";
                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    try
                    {
                        
                        var reader =  cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            users.Add(new UserResponce()
                            {
                                Id = UserId,
                                FirstName = reader["FirstName"].ToString(),
                                SecondName = reader["SecondName"].ToString()
                            });
                        }


                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return users;
        }
    }
}
