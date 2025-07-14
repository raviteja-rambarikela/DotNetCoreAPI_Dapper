using DapperApiDemo.Models;

namespace DapperApi.IRepositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsername(string username);
        Task<int> Register(User user);
        Task<bool> UpdatePassword(int userId, string newPassword);
    }
}
