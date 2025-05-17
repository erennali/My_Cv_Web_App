using ErenAliKocaCV.Models;

namespace ErenAliKocaCV.Services.Interfaces
{
    public interface IProjectService
    {
        IEnumerable<Project> GetAllProjects();
        Project GetProjectById(int id);
        bool AddProject(Project project);
        bool UpdateProject(Project project);
        bool DeleteProject(int id);
    }
} 