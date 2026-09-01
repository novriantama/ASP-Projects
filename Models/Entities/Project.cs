using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPProjects.Models.Entities;

[Table("projects")]
public class Project
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("project_name")]
    public string ProjectName { get; set; } = string.Empty;

    [Column("description", TypeName = "nvarchar(max)")]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "On Progress";

    [Column("start_date", TypeName = "date")]
    public DateOnly? StartDate { get; set; }

    [Column("end_date", TypeName = "date")]
    public DateOnly? EndDate { get; set; }

    [Column("progress_percentage", TypeName = "decimal(5,2)")]
    public decimal ProgressPercentage { get; set; } = 0.00m;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
