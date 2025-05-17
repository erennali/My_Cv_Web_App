using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ErenAliKocaCV.Services.Implementations
{
    public class PersonalInfoService : IPersonalInfoService
    {
        private readonly ApplicationDbContext _context;

        public PersonalInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public PersonalInfo GetPersonalInfo()
        {
            return _context.PersonalInfos.FirstOrDefault();
        }

        public bool UpdatePersonalInfo(PersonalInfo personalInfo)
        {
            var existing = _context.PersonalInfos.FirstOrDefault();
            
            if (existing == null)
            {
                _context.PersonalInfos.Add(personalInfo);
            }
            else
            {
                _context.Entry(existing).CurrentValues.SetValues(personalInfo);
            }
            
            return _context.SaveChanges() > 0;
        }
    }
} 