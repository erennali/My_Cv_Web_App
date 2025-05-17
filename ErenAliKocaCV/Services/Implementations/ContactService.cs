using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ErenAliKocaCV.Services.Implementations
{
    public class ContactService : IContactService
    {
        private readonly ApplicationDbContext _context;

        public ContactService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ContactMessage> GetAllContactMessages()
        {
            return _context.ContactMessages.OrderByDescending(m => m.DateSent).ToList();
        }

        public ContactMessage GetContactMessageById(int id)
        {
            return _context.ContactMessages.FirstOrDefault(m => m.Id == id);
        }

        public bool SaveContactMessage(ContactMessage message)
        {
            _context.ContactMessages.Add(message);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteContactMessage(int id)
        {
            var message = _context.ContactMessages.FirstOrDefault(m => m.Id == id);
            if (message == null)
                return false;
                
            _context.ContactMessages.Remove(message);
            return _context.SaveChanges() > 0;
        }
    }
} 