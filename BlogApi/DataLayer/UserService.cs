using BlogApi.Models;
using BlogApi.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using BlogApi.Helper;
using Microsoft.Data.Sqlite;

namespace BlogApi.DataLayer
{
   
    public class UserService : IUserService
    {
        private IConfiguration _configuration;
       
        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;           
        }

        public User Autheticate(UserRequest userRequest)
        {
            User user = null;
            using (SqliteConnection conn = new SqliteConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                using (SqliteCommand cmd = new SqliteCommand("SELECT * FROM Users WHERE Email = @Email;", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", userRequest.Email);
                    //cmd.Parameters.AddWithValue("@Password", userRequest.Password);

                    try
                    {
                        
                        var reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            DataTable table = new DataTable();
                            table.Load(reader);
                            reader.Close();
                            var userRow = table.Rows[0];

                            bool isValidPassword = BCrypt.Net.BCrypt.Verify(userRequest.Password, (string)userRow["UserPassword"]);
                            if (!isValidPassword)
                                return null;
                            
                            user = new User()
                            {
                                Id = Convert.ToInt32(userRow["Id"].ToString()),
                                FirstName = userRow["FirstName"].ToString(),
                                Email = (string)userRow["Email"].ToString(),
                                Role = (string)userRow["RoleName"].ToString()
                            };
                            
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            return user;
        }
        

        public int Register(UserRegister userRegister)
        {
            int Id = 0;
            var flag = CheckMail(userRegister.Email);

            if(flag == true)
            {
                return 0;
            }
            using (SqliteConnection conn = new SqliteConnection("DataSource = BlogApiSqlite.db"))
            {
                if (conn.State == ConnectionState.Closed)
                   conn.Open();
                using (SqliteCommand cmd = new SqliteCommand("INSERT INTO Users(FirstName, SecondName, Email, UserPassword) " +
                    "VALUES (@FirstName, @SecondName, @Email, @Password);" +
                    "SELECT last_insert_rowid();", conn))
                {
                    userRegister.Password = BCrypt.Net.BCrypt.HashPassword(userRegister.Password);

                    cmd.Parameters.AddWithValue("@FirstName", userRegister.FirstName);
                    cmd.Parameters.AddWithValue("@SecondName", userRegister.SecondName);
                    cmd.Parameters.AddWithValue("@Email", userRegister.Email);
                    cmd.Parameters.AddWithValue("@Password", userRegister.Password);

                    try
                    {
                        var UserId = cmd.ExecuteScalar();
                        Id = Int32.Parse(UserId.ToString());
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return Id;
        }

        private bool CheckMail(string mailString)
        {
            bool flag = false;
            using (SqliteConnection conn = new SqliteConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                using (SqliteCommand cmd = new SqliteCommand("Select Count(*) from Users Where Email = @Email;", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", mailString);
                    try
                    {

                        var Count = cmd.ExecuteScalar();
                        var Result = Convert.ToInt32(Count.ToString());
                        if (Result != 0)
                            return flag = true;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return flag;
        }
        
    }
}
