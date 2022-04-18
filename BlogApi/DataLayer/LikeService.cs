using BlogApi.Models.LikesModels;
using BlogApi.Services;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.DataLayer
{
    public class LikeService : ILikeService
    {
        private string _config;

        public LikeService(IConfiguration configuration) =>
            _config = configuration.GetConnectionString("DefaultConnection");

        public int GetCountLikeByPostId(Likes model)
        {
            int Result = 0;
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string QueryString = "SELECT Count(*) FROM Likes WHERE PostId = @PostId;";
                using (SqliteCommand cmd = new SqliteCommand(QueryString, conn))
                {
                    cmd.Parameters.AddWithValue("@PostId", model.PostId);

                    try
                    {
                       
                        var Count = cmd.ExecuteScalar();                        
                        Result = Convert.ToInt32(Count.ToString());
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return Result;
        }

        public bool LikeCheck(int UserId, Likes model)
        {
            bool Result = false;
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string QueryString = "SELECT Count(*) FROM Likes WHERE UserId = @UserId AND PostId = @PostId;";
                using (SqliteCommand cmd = new SqliteCommand(QueryString, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@PostId", model.PostId);

                    
                    try
                    {
                        
                        var Count =  cmd.ExecuteScalar();                        
                        if (Convert.ToInt32(Count.ToString()) == 1)
                        {                            
                            UnLikePost(UserId, model);  
                            return true;
                        }

                        LikePost(UserId, model);
                    }
                    catch (Exception ex)
                    {

                    }
                }


            }
            
            return Result;
        }


        public void LikePost(int UserId, Likes model)
        {
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string CommandString = "INSERT INTO Likes (PostId, UserId) " +
                    "VALUES(@PostId, @UserId);" +
                    "SELECT last_insert_rowid();";
                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@PostId", model.PostId);

                    try
                    {
                        var Id = cmd.ExecuteScalar();

                        model.LikeId = Int32.Parse(Id.ToString());
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        public void UnLikePost(int UserId, Likes model)
        {            
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string CommandString = "DELETE FROM Likes " +
                    "WHERE UserId = @UserId AND PostId = @PostId;";
                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {                    
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@PostId", model.PostId);

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
}
