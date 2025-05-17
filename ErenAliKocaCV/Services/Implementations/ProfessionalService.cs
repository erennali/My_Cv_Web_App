using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ErenAliKocaCV.Services.Implementations
{
    public class ProfessionalService : IProfessionalService
    {
        private readonly ApplicationDbContext _context;

        public ProfessionalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Service> GetAllServices()
        {
            return _context.Services.OrderBy(s => s.Title).ToList();
        }

        public Service GetServiceById(int id)
        {
            return _context.Services.FirstOrDefault(s => s.Id == id);
        }

        public bool AddService(Service service)
        {
            _context.Services.Add(service);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateService(Service service)
        {
            _context.Entry(service).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }

        public bool DeleteService(int id)
        {
            var service = _context.Services.FirstOrDefault(s => s.Id == id);
            if (service == null)
                return false;
                
            _context.Services.Remove(service);
            return _context.SaveChanges() > 0;
        }
    }
} 