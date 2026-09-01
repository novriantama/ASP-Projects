using ASPProjects.Data.Repositories;
using ASPProjects.Models.DTOs;

namespace ASPProjects.Business.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IIdProtector _idProtector;

    public UserService(IUserRepository userRepository, IIdProtector idProtector)
    {
        _userRepository = userRepository;
        _idProtector = idProtector;
    }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        return new UserDto
        {
            UserId = _idProtector.Encode(user.UserId),
            Username = user.Username,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<bool> UpdateRoleAsync(string userId, UpdateRoleDto updateRoleDto)
    {
        if (string.IsNullOrWhiteSpace(userId) || !_idProtector.TryDecode(userId, out var userIdInt))
        {
            throw new ArgumentException("Invalid encrypted user ID format.");
        }

        var user = await _userRepository.GetByIdAsync(userIdInt);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        return await _userRepository.UpdateRoleAsync(user.UserId, updateRoleDto.Role);
    }
}