using ASPProjects.Models.Entities;

namespace ASPProjects.Data.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<User> AddAsync(User user);
    Task<bool> UpdateRoleAsync(int userId, string role);
}
