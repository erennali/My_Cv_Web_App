using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ErenAliKocaCV.Services.Implementations
{
    public class ExperienceService : IExperienceService
    {
        private readonly ApplicationDbContext _context;

        public ExperienceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Experience> GetAllExperience()
        {
            return _context.Experiences.OrderByDescending(e => e.StartDate).ToList();
        }

        public Experience GetExperienceById(int id)
        {
            return _context.Experiences.FirstOrDefault(e => e.Id == id);
        }

        public bool AddExperience(Experience experience)
        {
            _context.Experiences.Add(experience);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateExperience(Experience experience)
        {
            _context.Entry(experience).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }

        public bool DeleteExperience(int id)
        {
            var experience = _context.Experiences.FirstOrDefault(e => e.Id == id);
            if (experience == null)
                return false;
                
            _context.Experiences.Remove(experience);
            return _context.SaveChanges() > 0;
        }
    }
} 