using System.ComponentModel.DataAnnotations;

namespace ASPProjects.Models.DTOs;

public class CreateProjectDto
{
    [Required]
    [MaxLength(255)]
    public string ProjectName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [RegularExpression("^(On Progress|Completed|Overdue)$", ErrorMessage = "Status must be 'On Progress', 'Completed', or 'Overdue'")]
    public string Status { get; set; } = "On Progress";

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    [Range(0.00, 100.00, ErrorMessage = "Progress percentage must be between 0 and 100")]
    public decimal ProgressPercentage { get; set; } = 0.00m;
}
