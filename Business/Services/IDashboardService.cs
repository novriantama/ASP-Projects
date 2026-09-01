using ASPProjects.Models.DTOs;

namespace ASPProjects.Business.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync();
}
