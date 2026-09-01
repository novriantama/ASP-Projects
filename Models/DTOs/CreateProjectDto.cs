namespace ASPProjects.Models.DTOs;

public class CreateProjectDto
{
    public string ProjectName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "On Progress";
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal ProgressPercentage { get; set; } = 0.00m;
}
