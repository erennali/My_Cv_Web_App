using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services;
using ErenAliKocaCV.Data;
using System;

namespace ErenAliKocaCV.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICVRepository _repository;
    private readonly ApplicationDbContext _context;
    private readonly IGitHubService _githubService;

    public HomeController(
        ILogger<HomeController> logger, 
        ICVRepository repository, 
        ApplicationDbContext context,
        IGitHubService githubService)
    {
        _logger = logger;
        _repository = repository;
        _context = context;
        _githubService = githubService;
    }

    public async Task<IActionResult> Index()
    {
        var personalInfo = _repository.GetPersonalInfo();
        ViewBag.PersonalInfo = personalInfo;
        
        ViewBag.Services = _repository.GetAllServices();
        ViewBag.Skills = _repository.GetAllSkills();
        ViewBag.Projects = _repository.GetAllProjects();
        ViewBag.Experiences = _repository.GetAllExperience();
        ViewBag.MediumArticles = _repository.GetAllMediumArticles();
        
        // GitHub kullanıcı adını alma
        string githubUsername = "erenalikoca";
        var configuration = HttpContext.RequestServices.GetService<IConfiguration>();
        if (configuration != null)
        {
            githubUsername = configuration["GitHub:Username"] ?? githubUsername;
        }
        
        // GitHub repolarını göster
        ViewBag.GitHubRepositories = await _githubService.GetEnrichedFeaturedRepositoriesAsync(githubUsername);
        
        // Proje sayısını güvenli bir şekilde al
        try 
        {
            int projectCount = 0;
            
            if (_context.Database.CanConnect() && _context.Projects != null)
            {
                projectCount = _context.Projects.Count();
            }
            
            ViewBag.ProjectCount = projectCount;
        }
        catch (Exception)
        {
            ViewBag.ProjectCount = 0;
        }
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}