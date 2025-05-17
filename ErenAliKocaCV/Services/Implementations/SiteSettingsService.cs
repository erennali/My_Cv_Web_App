using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ErenAliKocaCV.Services.Implementations
{
    public class SiteSettingsService : ISiteSettingsService
    {
        private readonly ApplicationDbContext _context;

        public SiteSettingsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public SiteSettings GetSiteSettings()
        {
            return _context.SiteSettings.FirstOrDefault();
        }

        public bool UpdateSiteSettings(SiteSettings settings)
        {
            var existing = _context.SiteSettings.FirstOrDefault();
            
            if (existing == null)
            {
                _context.SiteSettings.Add(settings);
            }
            else
            {
                _context.Entry(existing).CurrentValues.SetValues(settings);
            }
            
            return _context.SaveChanges() > 0;
        }
    }
} 