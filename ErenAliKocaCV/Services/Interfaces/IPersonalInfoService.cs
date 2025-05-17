using ErenAliKocaCV.Models;

namespace ErenAliKocaCV.Services.Interfaces
{
    public interface IPersonalInfoService
    {
        PersonalInfo GetPersonalInfo();
        bool UpdatePersonalInfo(PersonalInfo personalInfo);
    }
} 