using ErenAliKocaCV.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    public class DashboardController : AdminControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get counts for dashboard statistics
            ViewBag.PersonalInfoCount = await _context.PersonalInfos.CountAsync();
            ViewBag.ExperienceCount = await _context.Experiences.CountAsync();
            ViewBag.SkillCount = await _context.Skills.CountAsync();
            ViewBag.ProjectCount = await _context.Projects.CountAsync();
            ViewBag.ServiceCount = await _context.Services.CountAsync();
            ViewBag.MessageCount = await _context.ContactMessages.CountAsync(m => !m.IsRead);
            
            // Get recent messages for dashboard
            ViewBag.Messages = await _context.ContactMessages
                .OrderByDescending(m => m.DateSent)
                .Take(5)
                .ToListAsync();
            
            // Get recent activity logs for audit trail if the table exists
            if (_context.Model.FindEntityType(typeof(ErenAliKocaCV.Models.ActivityLog)) != null)
            {
                ViewBag.ActivityLogs = await _context.Set<ErenAliKocaCV.Models.ActivityLog>()
                    .OrderByDescending(a => a.Timestamp)
                    .Take(10)
                    .ToListAsync();
            }

            // Get site statistics
            var today = DateTime.Today;
            ViewBag.TodayMessages = await _context.ContactMessages
                .CountAsync(m => m.DateSent.Date == today);
            
            // Get admin users count
            ViewBag.AdminUsersCount = await _context.AdminUsers.CountAsync();
            
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetStatistics(int days = 30)
        {
            var endDate = DateTime.Today.AddDays(1);
            var startDate = endDate.AddDays(-days);
            
            var messageStats = await _context.ContactMessages
                .Where(m => m.DateSent >= startDate && m.DateSent < endDate)
                .GroupBy(m => m.DateSent.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();
            
            return Json(new { 
                success = true,
                messageStats = messageStats.Select(m => new { date = m.Date.ToString("yyyy-MM-dd"), count = m.Count }).ToList()
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
                ServerTimeUtc = DateTime.UtcNow
            };
            
            return Json(systemInfo);
        }
    }
} 