using ASPProjects.Models.DTOs;

namespace ASPProjects.Business.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
    Task<ProjectDto?> GetProjectByIdAsync(string encryptedId);
    Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto);
    Task<ProjectDto?> UpdateProjectAsync(string encryptedId, UpdateProjectDto dto);
    Task<bool> DeleteProjectAsync(string encryptedId);
}
