
using DapperApiDemo.Models;

namespace DapperApiDemo.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsername(string username);
        Task<int> Register(User user);
        Task<bool> UpdatePassword(int userId, string newPassword);
    }
}
