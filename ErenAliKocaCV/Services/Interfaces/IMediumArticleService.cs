using ErenAliKocaCV.Models;

namespace ErenAliKocaCV.Services.Interfaces
{
    public interface IMediumArticleService
    {
        IEnumerable<MediumArticle> GetAllMediumArticles();
        MediumArticle GetMediumArticleById(int id);
        bool AddMediumArticle(MediumArticle article);
        bool UpdateMediumArticle(MediumArticle article);
        bool DeleteMediumArticle(int id);
    }
} 