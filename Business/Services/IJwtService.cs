using ASPProjects.Models.Entities;

namespace ASPProjects.Business.Services;

public interface IJwtService
{
    (string token, DateTime expiresAt) GenerateToken(User user);
}
