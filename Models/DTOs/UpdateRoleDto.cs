using System.ComponentModel.DataAnnotations;

namespace ASPProjects.Models.DTOs;

public class UpdateRoleDto
{
    [Required]
    [RegularExpression("^(Admin|User)$", ErrorMessage = "Role must be Admin or User")]
    public string Role { get; set; } = string.Empty;
}