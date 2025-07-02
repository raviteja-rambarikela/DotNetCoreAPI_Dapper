
using Dapper;
using Microsoft.Data.SqlClient;
using DapperApiDemo.Models;

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

        public async Task<User?> GetByUsername(string username)
        {
            var sql = "SELECT * FROM Users WHERE Username = @Username";
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
        }

        public async Task<int> Register(User user)
        {
            var sql = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(sql, user);
        }
    }
}
