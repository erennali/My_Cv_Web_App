using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Services;
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
        private readonly ICVRepository _repository;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ProjectsController(ICVRepository repository, ApplicationDbContext context, IConfiguration configuration)
        {
            _repository = repository;
            _context = context;
            _configuration = configuration;
        }
        
        public IActionResult Index()
        {
            try
            {
                var projects = _repository.GetAllProjects();
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