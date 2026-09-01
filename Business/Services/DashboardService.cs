using ASPProjects.Data.Repositories;
using ASPProjects.Models.DTOs;

namespace ASPProjects.Business.Services;

public class DashboardService : IDashboardService
{
    private readonly IProjectRepository _projectRepository;

    public DashboardService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var projects = (await _projectRepository.GetAllAsync()).ToList();

        var totalProject = projects.Count;
        if (totalProject == 0)
        {
            return new DashboardSummaryDto
            {
                TotalProject = 0,
                OnProgress = 0,
                Completed = 0,
                Overdue = 0,
                ProgressPercentage = 0.00m
            };
        }

        var onProgress = projects.Count(p => string.Equals(p.Status, "On Progress", StringComparison.OrdinalIgnoreCase));
        var completed = projects.Count(p => string.Equals(p.Status, "Completed", StringComparison.OrdinalIgnoreCase));
        var overdue = projects.Count(p => string.Equals(p.Status, "Overdue", StringComparison.OrdinalIgnoreCase));

        var avgProgress = projects.Average(p => p.ProgressPercentage);

        return new DashboardSummaryDto
        {
            TotalProject = totalProject,
            OnProgress = onProgress,
            Completed = completed,
            Overdue = overdue,
            ProgressPercentage = Math.Round(avgProgress, 2)
        };
    }
}
