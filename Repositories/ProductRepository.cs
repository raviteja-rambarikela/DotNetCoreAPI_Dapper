
using Dapper;
using Microsoft.Data.SqlClient;
using DapperApiDemo.Models;

namespace DapperApiDemo.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public ProductRepository(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Product>> GetAll()
        {
            var sql = "SELECT * FROM Products";
            using var connection = CreateConnection();
            return await connection.QueryAsync<Product>(sql);
        }

        public async Task<Product?> GetById(int id)
        {
            var sql = "SELECT * FROM Products WHERE ProductId = @Id";
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
        }

        public async Task<int> Create(Product product)
        {
            var sql = "INSERT INTO Products (Name, Price) VALUES (@Name, @Price)";
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(sql, product);
        }

        public async Task<int> Update(Product product)
        {
            var sql = "UPDATE Products SET Name = @Name, Price = @Price WHERE ProductId = @ProductId";
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(sql, product);
        }

        public async Task<int> Delete(int id)
        {
            var sql = "DELETE FROM Products WHERE ProductId = @Id";
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
