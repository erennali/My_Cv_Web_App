using ErenAliKocaCV.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : AdminControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPersonalInfoService _personalInfoService;
        private readonly IProjectService _projectService;
        private readonly ISkillService _skillService;
        private readonly IExperienceService _experienceService;

        public DashboardController(
            ApplicationDbContext context, 
            IPersonalInfoService personalInfoService,
            IProjectService projectService,
            ISkillService skillService,
            IExperienceService experienceService)
        {
            _context = context;
            _personalInfoService = personalInfoService;
            _projectService = projectService;
            _skillService = skillService;
            _experienceService = experienceService;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            
            // Temel istatistikler
            ViewBag.PersonalInfoCount = await _context.PersonalInfos.CountAsync();
            ViewBag.ExperienceCount = await _context.Experiences.CountAsync();
            ViewBag.SkillCount = await _context.Skills.CountAsync();
            ViewBag.ProjectCount = await _context.Projects.CountAsync();
            ViewBag.ServiceCount = await _context.Services.CountAsync();
            ViewBag.UnreadMessageCount = await _context.ContactMessages.CountAsync(m => !m.IsRead);
            ViewBag.TotalMessageCount = await _context.ContactMessages.CountAsync();
            
            // Zaman çerçevesine göre mesaj istatistikleri
            ViewBag.TodayMessages = await _context.ContactMessages
                .CountAsync(m => m.DateSent.Date == today);
            ViewBag.WeekMessages = await _context.ContactMessages
                .CountAsync(m => m.DateSent >= startOfWeek);
            ViewBag.MonthMessages = await _context.ContactMessages
                .CountAsync(m => m.DateSent >= startOfMonth);
            
            // Son mesajlar
            ViewBag.Messages = await _context.ContactMessages
                .OrderByDescending(m => m.DateSent)
                .Take(5)
                .ToListAsync();
            
            // Aktivite logları
            if (_context.Model.FindEntityType(typeof(ActivityLog)) != null)
            {
                ViewBag.ActivityLogs = await _context.Set<ActivityLog>()
                    .OrderByDescending(a => a.Timestamp)
                    .Take(10)
                    .ToListAsync();
                
                // Login istatistikleri
                ViewBag.TodayLogins = await _context.Set<ActivityLog>()
                    .CountAsync(a => a.Timestamp.Date == today && a.Action == "Login" && a.IsSuccess);
                
                // Son başarısız oturum açma girişimleri
                ViewBag.FailedLogins = await _context.Set<ActivityLog>()
                    .Where(a => a.Action == "Login" && !a.IsSuccess)
                    .OrderByDescending(a => a.Timestamp)
                    .Take(5)
                    .ToListAsync();
                
                // Son aktivite türleri ve sayıları
                var actionStats = await _context.Set<ActivityLog>()
                    .Where(a => a.Timestamp >= today.AddDays(-30))
                    .GroupBy(a => a.Action)
                    .Select(g => new { Action = g.Key, Count = g.Count() })
                    .ToListAsync();
                
                ViewBag.ActionStats = actionStats;
            }
            
            // CV durumu ve tamamlanma oranı
            var cvStatus = new Dictionary<string, bool>
            {
                { "Kişisel Bilgiler", ViewBag.PersonalInfoCount > 0 },
                { "Deneyim", ViewBag.ExperienceCount > 0 },
                { "Yetenekler", ViewBag.SkillCount > 0 },
                { "Projeler", ViewBag.ProjectCount > 0 },
                { "Servisler", ViewBag.ServiceCount > 0 },
            };
            
            ViewBag.CVStatus = cvStatus;
            ViewBag.CVCompletionPercent = cvStatus.Count(s => s.Value) * 100 / cvStatus.Count;
            
            // Son güncelleme tarihleri
            ViewBag.LastPersonalInfoUpdate = await GetLastUpdateDate(_context.PersonalInfos);
            ViewBag.LastExperienceUpdate = await GetLastUpdateDate(_context.Experiences);
            ViewBag.LastProjectUpdate = await GetLastUpdateDate(_context.Projects);
            ViewBag.LastSkillUpdate = await GetLastUpdateDate(_context.Skills);
            
            // Skill kategorileri ve dağılımı
            var skillCategories = _skillService.GetAllSkills()
                .GroupBy(s => s.Category ?? "Diğer")
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();
            
            ViewBag.SkillCategories = skillCategories;
            
            // Proje kategorileri ve dağılımı
            var projectCategories = _projectService.GetAllProjects()
                .GroupBy(p => p.Category ?? "Diğer")
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();
            
            ViewBag.ProjectCategories = projectCategories;
            
            // Admin kullanıcı sayısı
            ViewBag.AdminUsersCount = await _context.AdminUsers.CountAsync();
            
            // Günlük/haftalık/aylık mesaj artış oranı
            var monthAgoMessages = await _context.ContactMessages
                .CountAsync(m => m.DateSent >= startOfMonth.AddMonths(-1) && m.DateSent < startOfMonth);
            
            ViewBag.MonthlyMessageGrowth = monthAgoMessages > 0
                ? ((double)ViewBag.MonthMessages / monthAgoMessages - 1) * 100
                : 100;
            
            // Sistem bilgileri
            ViewBag.SystemInfo = new
            {
                OsVersion = Environment.OSVersion.ToString(),
                MachineName = Environment.MachineName,
                ProcessorCount = Environment.ProcessorCount,
                DotNetVersion = Environment.Version.ToString(),
                ServerTime = DateTime.Now,
                ServerTimeUtc = DateTime.UtcNow,
                Runtime = GetProcessUptime()
            };
            
            return View();
        }
        
        private async Task<DateTime?> GetLastUpdateDate<T>(DbSet<T> dbSet) where T : class
        {
            if (typeof(T).GetProperty("UpdatedAt") != null)
            {
                // Eğer entity'de UpdatedAt varsa onu kullan
                return await dbSet
                    .Select(e => EF.Property<DateTime?>(e, "UpdatedAt"))
                    .OrderByDescending(d => d)
                    .FirstOrDefaultAsync();
            }
            
            // Diğer durumda ID'ye göre en son kaydı bul
            var lastRecord = await dbSet
                .OrderBy(e => EF.Property<object>(e, "Id"))  // Assuming Id is the primary key
                .LastOrDefaultAsync();
            
            return lastRecord != null ? DateTime.Now : null;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetStatistics(int days = 30)
        {
            var endDate = DateTime.Today.AddDays(1);
            var startDate = endDate.AddDays(-days);
            
            // Mesaj istatistikleri
            var messageStats = await _context.ContactMessages
                .Where(m => m.DateSent >= startDate && m.DateSent < endDate)
                .GroupBy(m => m.DateSent.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();
            
            // Aktivite istatistikleri
            var activityStats = new List<dynamic>();
            if (_context.Model.FindEntityType(typeof(ActivityLog)) != null)
            {
                var stats = await _context.Set<ActivityLog>()
                    .Where(a => a.Timestamp >= startDate && a.Timestamp < endDate)
                    .GroupBy(a => a.Timestamp.Date)
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .ToListAsync();
                
                activityStats = stats.Cast<dynamic>().ToList();
            }
            
            return Json(new { 
                success = true,
                messageStats = messageStats.Select(m => new { 
                    date = m.Date.ToString("yyyy-MM-dd"), 
                    count = m.Count 
                }).ToList(),
                activityStats = activityStats.Select(a => new { 
                    date = a.Date.ToString("yyyy-MM-dd"),
                    count = a.Count
                }).ToList()
            });
        }
        
        [HttpGet]
        public IActionResult SystemInfo()
        {
            var systemInfo = new
            {
                OsVersion = Environment.OSVersion.ToString(),
                MachineName = Environment.MachineName,
                ProcessorCount = Environment.ProcessorCount,
                DotNetVersion = Environment.Version.ToString(),
                ServerTime = DateTime.Now,
                ServerTimeUtc = DateTime.UtcNow,
                Runtime = GetProcessUptime()
            };
            
            return Json(systemInfo);
        }
        
        private string GetProcessUptime()
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            var uptime = DateTime.Now - process.StartTime;
            
            if (uptime.TotalDays >= 1)
                return $"{(int)uptime.TotalDays} gün {uptime.Hours} saat {uptime.Minutes} dakika";
            else if (uptime.TotalHours >= 1)
                return $"{(int)uptime.TotalHours} saat {uptime.Minutes} dakika";
            else
                return $"{uptime.Minutes} dakika {uptime.Seconds} saniye";
        }
    }
} 