using ErenAliKocaCV.Models;

namespace ErenAliKocaCV.Services.Interfaces
{
    public interface ISiteSettingsService
    {
        SiteSettings GetSiteSettings();
        bool UpdateSiteSettings(SiteSettings settings);
    }
} 