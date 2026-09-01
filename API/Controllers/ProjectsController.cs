using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ASPProjects.Business.Services;
using ASPProjects.Models.DTOs;

namespace ASPProjects.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    // GET /api/projects
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll()
    {
        var projects = await _projectService.GetAllProjectsAsync();
        return Ok(projects);
    }

    // GET /api/projects/{encryptedid}
    [HttpGet("{encryptedid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetById(string encryptedid)
    {
        if (string.IsNullOrWhiteSpace(encryptedid))
        {
            return BadRequest(new { message = "Encrypted project ID cannot be empty." });
        }

        var project = await _projectService.GetProjectByIdAsync(encryptedid);
        if (project == null)
        {
            return NotFound(new { message = "Project not found or invalid ID format." });
        }

        return Ok(project);
    }

    // POST /api/projects
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdProject = await _projectService.CreateProjectAsync(dto);
        return CreatedAtAction(nameof(GetById), new { encryptedid = createdProject.Id }, createdProject);
    }

    // PUT /api/projects/{encryptedid}
    [HttpPut("{encryptedid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> Update(string encryptedid, [FromBody] UpdateProjectDto dto)
    {
        if (string.IsNullOrWhiteSpace(encryptedid))
        {
            return BadRequest(new { message = "Encrypted project ID cannot be empty." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedProject = await _projectService.UpdateProjectAsync(encryptedid, dto);
        if (updatedProject == null)
        {
            return NotFound(new { message = "Project not found or invalid ID format." });
        }

        return Ok(updatedProject);
    }

    // DELETE /api/projects/{encryptedid}
    [HttpDelete("{encryptedid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string encryptedid)
    {
        if (string.IsNullOrWhiteSpace(encryptedid))
        {
            return BadRequest(new { message = "Encrypted project ID cannot be empty." });
        }

        var success = await _projectService.DeleteProjectAsync(encryptedid);
        if (!success)
        {
            return NotFound(new { message = "Project not found or invalid ID format." });
        }

        return NoContent();
    }
}
