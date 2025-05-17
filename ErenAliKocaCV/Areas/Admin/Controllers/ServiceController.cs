using ErenAliKocaCV.Models;
using ErenAliKocaCV.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ErenAliKocaCV.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ErenAliKocaCV.Areas.Admin.Controllers
{
    public class ServiceController : AdminControllerBase
    {
        private readonly IProfessionalService _professionalService;
        private readonly ApplicationDbContext _context;

        public ServiceController(IProfessionalService professionalService, ApplicationDbContext context)
        {
            _professionalService = professionalService;
            _context = context;
        }

        // GET: Admin/Service
        public IActionResult Index()
        {
            return View(_professionalService.GetAllServices());
        }

        // GET: Admin/Service/UpdateAllIcons
        public IActionResult UpdateAllIcons()
        {
            try
            {
                var services = _context.Services.ToList();
                int count = 0;
                
                foreach (var service in services)
                {
                    service.IconName = "flaticon-analysis";
                    service.IconClass = "flaticon-analysis";
                    count++;
                }
                
                _context.SaveChanges();
                TempData["SuccessMessage"] = $"Başarıyla {count} servisin ikonları 'flaticon-analysis' olarak güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"İkonları güncellerken hata: {ex.Message}";
                Debug.WriteLine($"Error updating icons: {ex.Message}");
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Service/Details/5
        public IActionResult Details(int id)
        {
            var service = _professionalService.GetServiceById(id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // GET: Admin/Service/Create
        public IActionResult Create()
        {
            // Initialize a new service with default values
            var service = new Service
            {
                IconName = "default-icon", // Set a default value for IconName
                Order = 0
            };
            return View(service);
        }

        // POST: Admin/Service/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Service service)
        {
            // Handle IconName and IconClass - make sure at least one is set
            if (string.IsNullOrEmpty(service.IconName) && string.IsNullOrEmpty(service.IconClass))
            {
                // If both are empty, use default flaticon
                service.IconName = "flaticon-analysis";
                
                // If using default flaticon, set default empty value for IconClass
                service.IconClass = "";
                
                // Remove IconClass validation errors
                ModelState.Remove("IconClass");
            }
            else if (!string.IsNullOrEmpty(service.IconClass))
            {
                // If IconClass is set, ensure IconName is cleared to avoid conflicts
                service.IconName = "";
                
                // Remove IconName validation errors if using FontAwesome
                ModelState.Remove("IconName");
            }
            else if (!string.IsNullOrEmpty(service.IconName))
            {
                // If IconName is set, ensure IconClass is cleared to avoid conflicts
                service.IconClass = "";
                
                // Remove IconClass validation errors if using Flaticon
                ModelState.Remove("IconClass");
            }
            
            // Detailed debugging to see what's being received
            Debug.WriteLine($"Create POST received - Service: Title={service.Title}, IconClass={service.IconClass}, IconName={service.IconName}, Order={service.Order}");
            
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                Debug.WriteLine($"Model state for {key}: Valid={state.ValidationState}, Value={state.RawValue}");
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    Debug.WriteLine($"Model is valid, attempting to add service");
                    if (_professionalService.AddService(service))
                    {
                        Debug.WriteLine("Service added successfully!");
                        TempData["SuccessMessage"] = "Service added successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        Debug.WriteLine("Error adding service - repository returned false");
                        ModelState.AddModelError("", "Error adding service.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception in Create: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }
                    ModelState.AddModelError("", $"Error adding service: {ex.Message}");
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine($"Model error: {error.ErrorMessage}");
                }
            }
            
            // Return to the form with the current service values
            return View(service);
        }

        // GET: Admin/Service/Edit/5
        public IActionResult Edit(int id)
        {
            var service = _professionalService.GetServiceById(id);
            if (service == null)
            {
                return NotFound();
            }
            
            return View(service);
        }

        // POST: Admin/Service/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Service service)
        {
            // Handle IconName and IconClass - make sure at least one is set
            if (string.IsNullOrEmpty(service.IconName) && string.IsNullOrEmpty(service.IconClass))
            {
                // If both are empty, use default flaticon
                service.IconName = "flaticon-analysis";
                
                // If using default flaticon, set default empty value for IconClass
                service.IconClass = "";
                
                // Remove IconClass validation errors
                ModelState.Remove("IconClass");
            }
            else if (!string.IsNullOrEmpty(service.IconClass))
            {
                // If IconClass is set, ensure IconName is cleared to avoid conflicts
                service.IconName = "";
                
                // Remove IconName validation errors if using FontAwesome
                ModelState.Remove("IconName");
            }
            else if (!string.IsNullOrEmpty(service.IconName))
            {
                // If IconName is set, ensure IconClass is cleared to avoid conflicts
                service.IconClass = "";
                
                // Remove IconClass validation errors if using Flaticon
                ModelState.Remove("IconClass");
            }

            if (id != service.Id)
            {
                Debug.WriteLine($"ID mismatch. Route ID: {id}, Model ID: {service.Id}");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Debug.WriteLine($"Attempting to update service: {service.Title}, ID: {service.Id}");
                if (_professionalService.UpdateService(service))
                {
                    Debug.WriteLine("Service updated successfully!");
                    TempData["SuccessMessage"] = "Service updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Debug.WriteLine("Error updating service - repository returned false");
                    ModelState.AddModelError("", "Error updating service.");
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine($"Model error: {error.ErrorMessage}");
                }
            }
            return View(service);
        }

        // GET: Admin/Service/Delete/5
        public IActionResult Delete(int id)
        {
            var service = _professionalService.GetServiceById(id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // POST: Admin/Service/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_professionalService.DeleteService(id))
            {
                TempData["SuccessMessage"] = "Service deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting service.";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}  
