using ErenAliKocaCV.Models;

namespace ErenAliKocaCV.Services.Interfaces
{
    public interface ICVFileService
    {
        CVFile GetActiveCVFile();
        IEnumerable<CVFile> GetAllCVFiles();
        bool AddCVFile(CVFile cvFile);
        bool SetCVFileActive(int id);
        bool DeleteCVFile(int id);
    }
} 