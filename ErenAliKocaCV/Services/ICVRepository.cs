using ErenAliKocaCV.Models;

namespace ErenAliKocaCV.Services
{
    public interface ICVRepository
    {
        // Personal Info
        PersonalInfo GetPersonalInfo();
        bool UpdatePersonalInfo(PersonalInfo personalInfo);
        
        
        
        // Experience
        IEnumerable<Experience> GetAllExperience();
        Experience GetExperienceById(int id);
        bool AddExperience(Experience experience);
        bool UpdateExperience(Experience experience);
        bool DeleteExperience(int id);
        
        // Skills
        IEnumerable<Skill> GetAllSkills();
        Skill GetSkillById(int id);
        bool AddSkill(Skill skill);
        bool UpdateSkill(Skill skill);
        bool DeleteSkill(int id);
        
        // Projects
        IEnumerable<Project> GetAllProjects();
        Project GetProjectById(int id);
        bool AddProject(Project project);
        bool UpdateProject(Project project);
        bool DeleteProject(int id);
        
        
        // Services
        IEnumerable<Service> GetAllServices();
        Service GetServiceById(int id);
        bool AddService(Service service);
        bool UpdateService(Service service);
        bool DeleteService(int id);
        
        // Medium Articles
        IEnumerable<MediumArticle> GetAllMediumArticles();
        MediumArticle GetMediumArticleById(int id);
        bool AddMediumArticle(MediumArticle article);
        bool UpdateMediumArticle(MediumArticle article);
        bool DeleteMediumArticle(int id);
        
        // Contact Messages
        IEnumerable<ContactMessage> GetAllContactMessages();
        ContactMessage GetContactMessageById(int id);
        bool SaveContactMessage(ContactMessage message);
        bool DeleteContactMessage(int id);
        
        // CV File
        CVFile GetActiveCVFile();
        IEnumerable<CVFile> GetAllCVFiles();
        bool AddCVFile(CVFile cvFile);
        bool SetCVFileActive(int id);
        bool DeleteCVFile(int id);
        
        // Site Settings
        SiteSettings GetSiteSettings();
        bool UpdateSiteSettings(SiteSettings settings);
        
        // Save changes
        bool SaveChanges();
    }
} 