using BlogApi.Models;
using BlogApi.Models.Admins;
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
    public class AdminService : IAdminServise
    {
        private string _config;

        public AdminService(IConfiguration configuration) =>
            _config = configuration.GetConnectionString("DefaultConnection");

        public IEnumerable<AdminResponsePerDate> GetPostPerDate(AdminsRequestsDate model)
        {
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string query = "SELECT Count(*) AS CountPosts FROM Posts WHERE CreateTime = @Date;";
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Date", model.Date);

                    

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var AdminResponse = new AdminResponsePerDate();

                        AdminResponse.Date = model.Date;
                        AdminResponse.CountPostPerday = int.Parse(reader["CountPosts"].ToString());

                        yield return AdminResponse;
                    }
                }
            }
        }

        public List<AdminsResponsePerTags> GetPostPerTags(AdminsRequestTags model)
        {
            List<AdminsResponsePerTags> tagResponse = new List<AdminsResponsePerTags>();
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                foreach (string tag in model.Tags)
                {
                    string query = "SELECT Count(*) AS CountPosts FROM Posts JOIN PostsTags ON PostsTags.PostId = Posts.Id JOIN Tags ON Tags.Id = PostsTags.TagsId AND Tags.title = @Tags;";
                    using (SqliteCommand cmd = new SqliteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Tags", tag);

                        try
                        {
                            var reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                tagResponse.Add(new AdminsResponsePerTags()
                                {
                                    Tags = tag,
                                    CountPosts = Convert.ToInt32(reader["CountPosts"].ToString())
                                });
                            }
                            
                        }
                        catch(Exception ex)
                        {

                        }
                        
                    }
                }               

            }
            return tagResponse;
        }

        public IEnumerable<AdminResponsePerUsers> GetPostPerUser(int UserId)
        {            
            using (SqliteConnection conn = new SqliteConnection(_config))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string query = "SELECT Id, FirstName, SecondName, (SELECT Count(*) FROM Posts WHERE UserId = @UserId) " +
                    "AS CountPost FROM Users;";
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);                    

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var AdminResponse = new AdminResponsePerUsers();

                        AdminResponse.Users = new List<UserResponce>();
                        AdminResponse.Users.Add(new UserResponce()
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            FirstName = reader["FirstName"].ToString(),
                            SecondName = reader["SecondName"].ToString()
                        });
                        AdminResponse.CountPostPerUser = int.Parse(reader["CountPost"].ToString());

                        yield return AdminResponse;
                    }
                }
            }

        }
    }
}
