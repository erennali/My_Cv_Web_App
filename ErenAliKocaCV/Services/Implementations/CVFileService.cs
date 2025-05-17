using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ErenAliKocaCV.Services.Implementations
{
    public class CVFileService : ICVFileService
    {
        private readonly ApplicationDbContext _context;

        public CVFileService(ApplicationDbContext context)
        {
            _context = context;
        }

        public CVFile GetActiveCVFile()
        {
            return _context.CVFiles.FirstOrDefault(f => f.IsActive);
        }

        public IEnumerable<CVFile> GetAllCVFiles()
        {
            return _context.CVFiles.OrderByDescending(f => f.IsActive).ToList();
        }

        public bool AddCVFile(CVFile cvFile)
        {
            _context.CVFiles.Add(cvFile);
            return _context.SaveChanges() > 0;
        }

        public bool SetCVFileActive(int id)
        {
            // First, set all CV files as inactive
            foreach (var file in _context.CVFiles)
            {
                file.IsActive = false;
            }
            
            // Then set the selected file as active
            var cvFile = _context.CVFiles.FirstOrDefault(f => f.Id == id);
            if (cvFile == null)
                return false;
                
            cvFile.IsActive = true;
            return _context.SaveChanges() > 0;
        }

        public bool DeleteCVFile(int id)
        {
            var cvFile = _context.CVFiles.FirstOrDefault(f => f.Id == id);
            if (cvFile == null)
                return false;
                
            // Don't allow deleting if it's the active file
            if (cvFile.IsActive)
                return false;
                
            _context.CVFiles.Remove(cvFile);
            return _context.SaveChanges() > 0;
        }
    }
} 