using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using ErenAliKocaCV.Data;
using System;

namespace ErenAliKocaCV.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPersonalInfoService _personalInfoService;
    private readonly IProfessionalService _professionalService;
    private readonly ISkillService _skillService;
    private readonly IProjectService _projectService;
    private readonly IExperienceService _experienceService;
    private readonly IMediumArticleService _mediumArticleService;
    private readonly IGitHubService _githubService;
    private readonly ApplicationDbContext _context;

    public HomeController(
        ILogger<HomeController> logger,
        IPersonalInfoService personalInfoService,
        IProfessionalService professionalService,
        ISkillService skillService,
        IProjectService projectService,
        IExperienceService experienceService,
        IMediumArticleService mediumArticleService,
        IGitHubService githubService,
        ApplicationDbContext context)
    {
        _logger = logger;
        _personalInfoService = personalInfoService;
        _professionalService = professionalService;
        _skillService = skillService;
        _projectService = projectService;
        _experienceService = experienceService;
        _mediumArticleService = mediumArticleService;
        _githubService = githubService;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var personalInfo = _personalInfoService.GetPersonalInfo();
        ViewBag.PersonalInfo = personalInfo;
        
        ViewBag.Services = _professionalService.GetAllServices();
        ViewBag.Skills = _skillService.GetAllSkills();
        ViewBag.Projects = _projectService.GetAllProjects();
        ViewBag.Experiences = _experienceService.GetAllExperience();
        ViewBag.MediumArticles = _mediumArticleService.GetAllMediumArticles();
        
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