namespace ASPProjects.Models.DTOs;

public class UpdateProjectDto
{
    public string ProjectName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal ProgressPercentage { get; set; }
}
