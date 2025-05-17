using ErenAliKocaCV.Models;

namespace ErenAliKocaCV.Services.Interfaces
{
    public interface IExperienceService
    {
        IEnumerable<Experience> GetAllExperience();
        Experience GetExperienceById(int id);
        bool AddExperience(Experience experience);
        bool UpdateExperience(Experience experience);
        bool DeleteExperience(int id);
    }
} 