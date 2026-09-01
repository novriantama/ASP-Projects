using ASPProjects.Models.DTOs;

namespace ASPProjects.Business.Services;

public interface IUserService
{
    Task<UserDto> GetUserByIdAsync(int id);
    Task<bool> UpdateRoleAsync(string userId, UpdateRoleDto updateRoleDto);
}
