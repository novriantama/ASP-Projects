using ASPProjects.Data.Repositories;
using ASPProjects.Models.DTOs;

namespace ASPProjects.Business.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ProjectDto?> GetProjectByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateProjectAsync(int id, UpdateProjectDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteProjectAsync(int id)
    {
        throw new NotImplementedException();
    }
}
