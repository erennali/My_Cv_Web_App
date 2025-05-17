using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ErenAliKocaCV.Services.Implementations
{
    public class MediumArticleService : IMediumArticleService
    {
        private readonly ApplicationDbContext _context;

        public MediumArticleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<MediumArticle> GetAllMediumArticles()
        {
            return _context.MediumArticles.OrderByDescending(a => a.PublishedDate).ToList();
        }

        public MediumArticle GetMediumArticleById(int id)
        {
            return _context.MediumArticles.FirstOrDefault(a => a.Id == id);
        }

        public bool AddMediumArticle(MediumArticle article)
        {
            _context.MediumArticles.Add(article);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateMediumArticle(MediumArticle article)
        {
            _context.Entry(article).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }

        public bool DeleteMediumArticle(int id)
        {
            var article = _context.MediumArticles.FirstOrDefault(a => a.Id == id);
            if (article == null)
                return false;
                
            _context.MediumArticles.Remove(article);
            return _context.SaveChanges() > 0;
        }
    }
} 