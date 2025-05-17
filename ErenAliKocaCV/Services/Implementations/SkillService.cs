using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ErenAliKocaCV.Services.Implementations
{
    public class SkillService : ISkillService
    {
        private readonly ApplicationDbContext _context;

        public SkillService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Skill> GetAllSkills()
        {
            return _context.Skills.OrderBy(s => s.Category).ThenBy(s => s.Name).ToList();
        }

        public Skill GetSkillById(int id)
        {
            return _context.Skills.FirstOrDefault(s => s.Id == id);
        }

        public bool AddSkill(Skill skill)
        {
            _context.Skills.Add(skill);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateSkill(Skill skill)
        {
            _context.Entry(skill).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }

        public bool DeleteSkill(int id)
        {
            var skill = _context.Skills.FirstOrDefault(s => s.Id == id);
            if (skill == null)
                return false;
                
            _context.Skills.Remove(skill);
            return _context.SaveChanges() > 0;
        }
    }
} 