using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ErenAliKocaCV.Services.Implementations
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Project> GetAllProjects()
        {
            return _context.Projects.OrderByDescending(p => p.CompletionDate).ToList();
        }

        public Project GetProjectById(int id)
        {
            return _context.Projects.FirstOrDefault(p => p.Id == id);
        }

        public bool AddProject(Project project)
        {
            _context.Projects.Add(project);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateProject(Project project)
        {
            _context.Entry(project).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }

        public bool DeleteProject(int id)
        {
            var project = _context.Projects.FirstOrDefault(p => p.Id == id);
            if (project == null)
                return false;
                
            _context.Projects.Remove(project);
            return _context.SaveChanges() > 0;
        }
    }
} 