
using Dapper;
using DapperApi.IRepositories;
using DapperApiDemo.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperApiDemo.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public UserRepository(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        //public async Task<User?> GetByUsername(string username)
        //{
        //    var sql = "SELECT * FROM Users WHERE Username = @Username";
        //    using var connection = CreateConnection();
        //    return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
        //}

        public async Task<User?> GetByUsername(string username)
        {
            var sp = "spGetUserByUsername";
            using var connection = CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<User>(
                sp,
                new { Username = username },
                commandType: CommandType.StoredProcedure);
        }


        //public async Task<int> Register(User user)
        //{
        //        var sql = @"
        //    INSERT INTO Users 
        //    (Username, Password, firstname, lastname, gender, phonenumber, IsActive, email)
        //    VALUES 
        //    (@Username, @Password, @firstname, @lastname, @gender, @phonenumber, @IsActive, @email)";
        //    using var connection = CreateConnection();
        //    return await connection.ExecuteAsync(sql, user);
        //}

        public async Task<int> Register(User user)
        {
            using var connection = CreateConnection();

            var parameters = new
            {
                user.Username,
                user.Password,
                user.firstname,
                user.lastname,
                user.gender,
                user.phonenumber,
                user.IsActive,
                user.email
            };

            // Run the stored procedure and get the new UserId or -1 if duplicate
            var userId = await connection.QuerySingleAsync<int>(
                "spRegisterUser",
                parameters,
                commandType: CommandType.StoredProcedure);

            return userId;
        }

        public async Task<bool> UpdatePassword(int userId, string newPassword)
        {
            var sql = "UPDATE Users SET Password = @Password WHERE UserId = @UserId";
            using var connection = CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new { Password = newPassword, UserId = userId });
            return rowsAffected > 0;
        }
    }
}
