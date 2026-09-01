namespace ASPProjects.Models.DTOs;

public class DashboardSummaryDto
{
    public int TotalProject { get; set; }
    public int OnProgress { get; set; }
    public int Completed { get; set; }
    public int Overdue { get; set; }
    public decimal ProgressPercentage { get; set; }
}
