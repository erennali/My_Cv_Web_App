using ErenAliKocaCV.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

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
        private readonly IMemoryCache _memoryCache;

        // Cache keys
        private const string StatisticsCacheKey = "DashboardStatistics";
        private const string RecentMessagesCacheKey = "RecentMessages";
        private const string ActivityLogsCacheKey = "ActivityLogs";
        private const string CvStatusCacheKey = "CvStatus";
        private const string SkillCategoriesCacheKey = "SkillCategories";
        private const string ProjectCategoriesCacheKey = "ProjectCategories";

        // Cache duration in minutes
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(15);

        public DashboardController(
            ApplicationDbContext context, 
            IPersonalInfoService personalInfoService,
            IProjectService projectService,
            ISkillService skillService,
            IExperienceService experienceService,
            IMemoryCache memoryCache)
        {
            _context = context;
            _personalInfoService = personalInfoService;
            _projectService = projectService;
            _skillService = skillService;
            _experienceService = experienceService;
            _memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            
            // Get or create dashboard statistics from cache
            if (!_memoryCache.TryGetValue(StatisticsCacheKey, out Dictionary<string, object> statistics))
            {
                statistics = new Dictionary<string, object>();
                
                // Temel istatistikler
                statistics["PersonalInfoCount"] = await _context.PersonalInfos.CountAsync();
                statistics["ExperienceCount"] = await _context.Experiences.CountAsync();
                statistics["SkillCount"] = await _context.Skills.CountAsync();
                statistics["ProjectCount"] = await _context.Projects.CountAsync();
                statistics["ServiceCount"] = await _context.Services.CountAsync();
                statistics["UnreadMessageCount"] = await _context.ContactMessages.CountAsync(m => !m.IsRead);
                statistics["TotalMessageCount"] = await _context.ContactMessages.CountAsync();
                
                // Zaman çerçevesine göre mesaj istatistikleri
                statistics["TodayMessages"] = await _context.ContactMessages
                    .CountAsync(m => m.DateSent.Date == today);
                statistics["WeekMessages"] = await _context.ContactMessages
                    .CountAsync(m => m.DateSent >= startOfWeek);
                statistics["MonthMessages"] = await _context.ContactMessages
                    .CountAsync(m => m.DateSent >= startOfMonth);
                
                // Son güncelleme tarihleri
                statistics["LastPersonalInfoUpdate"] = await GetLastUpdateDate(_context.PersonalInfos);
                statistics["LastExperienceUpdate"] = await GetLastUpdateDate(_context.Experiences);
                statistics["LastProjectUpdate"] = await GetLastUpdateDate(_context.Projects);
                statistics["LastSkillUpdate"] = await GetLastUpdateDate(_context.Skills);
                
                // Günlük/haftalık/aylık mesaj artış oranı
                var monthAgoMessages = await _context.ContactMessages
                    .CountAsync(m => m.DateSent >= startOfMonth.AddMonths(-1) && m.DateSent < startOfMonth);
                
                var monthMessages = (int)statistics["MonthMessages"];
                statistics["MonthlyMessageGrowth"] = monthAgoMessages > 0
                    ? ((double)monthMessages / monthAgoMessages - 1) * 100
                    : 100;
                
                // Admin kullanıcı sayısı
                statistics["AdminUsersCount"] = await _context.AdminUsers.CountAsync();
                
                // System information (always fresh)
                statistics["SystemInfo"] = new
                {
                    OsVersion = Environment.OSVersion.ToString(),
                    MachineName = Environment.MachineName,
                    ProcessorCount = Environment.ProcessorCount,
                    DotNetVersion = Environment.Version.ToString(),
                    ServerTime = DateTime.Now,
                    ServerTimeUtc = DateTime.UtcNow,
                    Runtime = GetProcessUptime()
                };
                
                // Cache the statistics
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(_cacheDuration)
                    .SetPriority(CacheItemPriority.High);
                
                _memoryCache.Set(StatisticsCacheKey, statistics, cacheEntryOptions);
            }
            
            // Set ViewBag values from cached statistics directly
            ViewBag.PersonalInfoCount = statistics["PersonalInfoCount"];
            ViewBag.ExperienceCount = statistics["ExperienceCount"];
            ViewBag.SkillCount = statistics["SkillCount"];
            ViewBag.ProjectCount = statistics["ProjectCount"];
            ViewBag.ServiceCount = statistics["ServiceCount"];
            ViewBag.UnreadMessageCount = statistics["UnreadMessageCount"];
            ViewBag.TotalMessageCount = statistics["TotalMessageCount"];
            ViewBag.TodayMessages = statistics["TodayMessages"];
            ViewBag.WeekMessages = statistics["WeekMessages"];
            ViewBag.MonthMessages = statistics["MonthMessages"];
            ViewBag.LastPersonalInfoUpdate = statistics["LastPersonalInfoUpdate"];
            ViewBag.LastExperienceUpdate = statistics["LastExperienceUpdate"];
            ViewBag.LastProjectUpdate = statistics["LastProjectUpdate"];
            ViewBag.LastSkillUpdate = statistics["LastSkillUpdate"];
            ViewBag.MonthlyMessageGrowth = statistics["MonthlyMessageGrowth"];
            ViewBag.AdminUsersCount = statistics["AdminUsersCount"];
            ViewBag.SystemInfo = statistics["SystemInfo"];
            
            // Get or create recent messages from cache
            if (!_memoryCache.TryGetValue(RecentMessagesCacheKey, out var recentMessages))
            {
                recentMessages = await _context.ContactMessages
                    .OrderByDescending(m => m.DateSent)
                    .Take(5)
                    .ToListAsync();
                
                _memoryCache.Set(RecentMessagesCacheKey, recentMessages, _cacheDuration);
            }
            ViewBag.Messages = recentMessages;
            
            // Activity logs if they exist (fresh data for admin dashboard)
            if (_context.Model.FindEntityType(typeof(ActivityLog)) != null)
            {
                // Get activity logs with caching
                if (!_memoryCache.TryGetValue(ActivityLogsCacheKey, out var activityLogs))
                {
                    activityLogs = await _context.Set<ActivityLog>()
                        .OrderByDescending(a => a.Timestamp)
                        .Take(10)
                        .ToListAsync();
                    
                    // Getting more activity log data
                    var todayLogins = await _context.Set<ActivityLog>()
                        .CountAsync(a => a.Timestamp.Date == today && a.Action == "Login" && a.IsSuccess);
                    
                    var failedLogins = await _context.Set<ActivityLog>()
                        .Where(a => a.Action == "Login" && !a.IsSuccess)
                        .OrderByDescending(a => a.Timestamp)
                        .Take(5)
                        .ToListAsync();
                    
                    var actionStats = await _context.Set<ActivityLog>()
                        .Where(a => a.Timestamp >= today.AddDays(-30))
                        .GroupBy(a => a.Action)
                        .Select(g => new { Action = g.Key, Count = g.Count() })
                        .ToListAsync();
                    
                    _memoryCache.Set(ActivityLogsCacheKey, activityLogs, TimeSpan.FromMinutes(5));
                    _memoryCache.Set("TodayLogins", todayLogins, TimeSpan.FromMinutes(5));
                    _memoryCache.Set("FailedLogins", failedLogins, TimeSpan.FromMinutes(5));
                    _memoryCache.Set("ActionStats", actionStats, TimeSpan.FromMinutes(5));
                }
                
                ViewBag.ActivityLogs = activityLogs;
                ViewBag.TodayLogins = _memoryCache.Get("TodayLogins");
                ViewBag.FailedLogins = _memoryCache.Get("FailedLogins");
                ViewBag.ActionStats = _memoryCache.Get("ActionStats");
            }
            
            // CV status (cached)
            if (!_memoryCache.TryGetValue(CvStatusCacheKey, out Dictionary<string, bool> cvStatus))
            {
                cvStatus = new Dictionary<string, bool>
                {
                    { "Kişisel Bilgiler", (int)statistics["PersonalInfoCount"] > 0 },
                    { "Deneyim", (int)statistics["ExperienceCount"] > 0 },
                    { "Yetenekler", (int)statistics["SkillCount"] > 0 },
                    { "Projeler", (int)statistics["ProjectCount"] > 0 },
                    { "Servisler", (int)statistics["ServiceCount"] > 0 },
                };
                
                _memoryCache.Set(CvStatusCacheKey, cvStatus, _cacheDuration);
            }
            
            ViewBag.CVStatus = cvStatus;
            ViewBag.CVCompletionPercent = cvStatus.Count(s => s.Value) * 100 / cvStatus.Count;
            
            // Get skill and project categories with caching
            if (!_memoryCache.TryGetValue(SkillCategoriesCacheKey, out var skillCategories))
            {
                skillCategories = _skillService.GetAllSkills()
                    .GroupBy(s => s.Category ?? "Diğer")
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToList();
                
                _memoryCache.Set(SkillCategoriesCacheKey, skillCategories, _cacheDuration);
            }
            ViewBag.SkillCategories = skillCategories;
            
            if (!_memoryCache.TryGetValue(ProjectCategoriesCacheKey, out var projectCategories))
            {
                projectCategories = _projectService.GetAllProjects()
                    .GroupBy(p => p.Category ?? "Diğer")
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToList();
                
                _memoryCache.Set(ProjectCategoriesCacheKey, projectCategories, _cacheDuration);
            }
            ViewBag.ProjectCategories = projectCategories;
            
            return View();
        }

        // Implement cache invalidation
        public IActionResult ClearCache()
        {
            _memoryCache.Remove(StatisticsCacheKey);
            _memoryCache.Remove(RecentMessagesCacheKey);
            _memoryCache.Remove(ActivityLogsCacheKey);
            _memoryCache.Remove(CvStatusCacheKey);
            _memoryCache.Remove(SkillCategoriesCacheKey);
            _memoryCache.Remove(ProjectCategoriesCacheKey);
            _memoryCache.Remove("TodayLogins");
            _memoryCache.Remove("FailedLogins");
            _memoryCache.Remove("ActionStats");
            
            return RedirectToAction(nameof(Index));
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
            string cacheKey = $"Statistics_{days}";
            
            if (!_memoryCache.TryGetValue(cacheKey, out object result))
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
                
                result = new { 
                    success = true,
                    messageStats = messageStats.Select(m => new { 
                        date = m.Date.ToString("yyyy-MM-dd"), 
                        count = m.Count 
                    }).ToList(),
                    activityStats = activityStats.Select(a => new { 
                        date = a.Date.ToString("yyyy-MM-dd"),
                        count = a.Count
                    }).ToList()
                };
                
                _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(30));
            }
            
            return Json(result);
        }
        
        [HttpGet]
        [ResponseCache(Duration = 60)]  // 1 minute server-side cache
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