using ASPProjects.Data.Repositories;
using ASPProjects.Models.DTOs;
using ASPProjects.Models.Entities;

namespace ASPProjects.Business.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IIdProtector _idProtector;

    public ProjectService(IProjectRepository projectRepository, IIdProtector idProtector)
    {
        _projectRepository = projectRepository;
        _idProtector = idProtector;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        return projects.Select(MapToDto);
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(string encryptedId)
    {
        if (!_idProtector.TryDecode(encryptedId, out var id))
        {
            return null;
        }

        var project = await _projectRepository.GetByIdAsync(id);
        return project == null ? null : MapToDto(project);
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto)
    {
        var project = new Project
        {
            ProjectName = dto.ProjectName,
            Description = dto.Description,
            Status = dto.Status,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            ProgressPercentage = dto.ProgressPercentage,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdProject = await _projectRepository.AddAsync(project);
        return MapToDto(createdProject);
    }

    public async Task<ProjectDto?> UpdateProjectAsync(string encryptedId, UpdateProjectDto dto)
    {
        if (!_idProtector.TryDecode(encryptedId, out var id))
        {
            return null;
        }

        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
        {
            return null;
        }

        project.ProjectName = dto.ProjectName;
        project.Description = dto.Description;
        project.Status = dto.Status;
        project.StartDate = dto.StartDate;
        project.EndDate = dto.EndDate;
        project.ProgressPercentage = dto.ProgressPercentage;
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project);
        return MapToDto(project);
    }

    public async Task<bool> DeleteProjectAsync(string encryptedId)
    {
        if (!_idProtector.TryDecode(encryptedId, out var id))
        {
            return false;
        }

        return await _projectRepository.DeleteAsync(id);
    }

    private ProjectDto MapToDto(Project project)
    {
        return new ProjectDto
        {
            Id = _idProtector.Encode(project.Id),
            ProjectName = project.ProjectName,
            Description = project.Description,
            Status = project.Status,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            ProgressPercentage = project.ProgressPercentage,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }
}
