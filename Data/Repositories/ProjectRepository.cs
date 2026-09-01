using ASPProjects.Database;
using ASPProjects.Models.Entities;

namespace ASPProjects.Data.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<Project>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Project?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Project> AddAsync(Project project)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Project project)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}
