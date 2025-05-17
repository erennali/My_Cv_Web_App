using ErenAliKocaCV.Models;

namespace ErenAliKocaCV.Services.Interfaces
{
    public interface IContactService
    {
        IEnumerable<ContactMessage> GetAllContactMessages();
        ContactMessage GetContactMessageById(int id);
        bool SaveContactMessage(ContactMessage message);
        bool DeleteContactMessage(int id);
    }
} 