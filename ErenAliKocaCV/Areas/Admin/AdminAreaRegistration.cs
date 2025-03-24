using Microsoft.AspNetCore.Mvc;

namespace ErenAliKocaCV.Areas.Admin
{
    [Area("Admin")]
    public class AdminAreaRegistration : AreaAttribute
    {
        public AdminAreaRegistration() : base("Admin")
        {
        }
    }
} 