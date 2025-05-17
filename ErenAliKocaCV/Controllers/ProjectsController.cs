using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Services.Interfaces;
using System;
using Microsoft.Data.SqlClient;
using ErenAliKocaCV.Data;
using Microsoft.EntityFrameworkCore;
using ErenAliKocaCV.Filters;

namespace ErenAliKocaCV.Controllers
{
    [RedirectToHome]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ProjectsController(IProjectService projectService, ApplicationDbContext context, IConfiguration configuration)
        {
            _projectService = projectService;
            _context = context;
            _configuration = configuration;
        }
        
        public IActionResult Index()
        {
            try
            {
                var projects = _projectService.GetAllProjects();
                ViewBag.Projects = projects;
                
                return View();
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Error in ProjectsController.Index: {ex.Message}");
                
                // Return to view with empty project list
                ViewBag.Projects = Array.Empty<Models.Project>();
                ViewBag.ErrorMessage = "An error occurred while loading projects.";
                
                return View();
            }
        }
    }
} 