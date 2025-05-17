using ErenAliKocaCV.Models;

namespace ErenAliKocaCV.Services.Interfaces
{
    public interface IProfessionalService
    {
        IEnumerable<Service> GetAllServices();
        Service GetServiceById(int id);
        bool AddService(Service service);
        bool UpdateService(Service service);
        bool DeleteService(int id);
    }
} 