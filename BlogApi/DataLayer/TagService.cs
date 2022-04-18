using BlogApi.Models.TagsModels;
using BlogApiSqlite.Services;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApiSqlite.DataLayer
{
    public class TagService : ITagService
    {
        private string _configuration;

        public TagService(IConfiguration configuration)
        {
            _configuration = configuration.GetConnectionString("DefaultConnection");
        }

        public bool DeleteTag(Tags model)
        {
            bool Return = false;
            using (SqliteConnection conn = new SqliteConnection(_configuration))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                
                string CommandString = "DELETE FROM Tags WHERE Id = @Id;";

                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", model.Id);
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
            return Return;
        }

        

        public IEnumerable<Tags> UpdateTag(Tags model)
        {
            using (SqliteConnection conn = new SqliteConnection(_configuration))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string CommandString = "Update Tags SET " +
                    "title = @TagName " +
                    "WHERE Id = @Id";

                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@TagName", model.TagName);

                    cmd.ExecuteNonQuery();

                    var tag = new Tags();

                    tag.Id = model.Id;
                    tag.TagName = model.TagName;

                    yield return tag;
                }
            }
        }

        public bool isTrue(int UserId, Tags model)
        {
            using (SqliteConnection conn = new SqliteConnection(_configuration))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string CommandString = "SELECT Count(*) AS CountPosts FROM PostsTags JOIN Posts ON PostsTags.PostId = Posts.Id AND PostsTags.TagsId = @Id AND Posts.UserId = @UserId;";

                using (SqliteCommand cmd = new SqliteCommand(CommandString, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    try
                    {                        
                        var Count = cmd.ExecuteScalar();
                        if (Convert.ToInt32(Count.ToString()) != 0)
                        {                            
                            return false;
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    
                }
            }
            return true;
        }
    }
}
