using ErenAliKocaCV.Models;

namespace ErenAliKocaCV.Services.Interfaces
{
    public interface ISkillService
    {
        IEnumerable<Skill> GetAllSkills();
        Skill GetSkillById(int id);
        bool AddSkill(Skill skill);
        bool UpdateSkill(Skill skill);
        bool DeleteSkill(int id);
    }
} 