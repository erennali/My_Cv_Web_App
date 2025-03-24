using ErenAliKocaCV.Data;
using ErenAliKocaCV.Models;
using Microsoft.EntityFrameworkCore;

namespace ErenAliKocaCV.Services
{
    public class CVRepository : ICVRepository
    {
        private readonly ApplicationDbContext _context;

        public CVRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Personal Info
        public PersonalInfo GetPersonalInfo()
        {
            return _context.PersonalInfos.FirstOrDefault();
        }

        public bool UpdatePersonalInfo(PersonalInfo personalInfo)
        {
            try
            {
                var existingInfo = _context.PersonalInfos.FirstOrDefault();
                if (existingInfo != null)
                {
                    _context.Entry(existingInfo).CurrentValues.SetValues(personalInfo);
                    return SaveChanges();
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Experience
        public IEnumerable<Experience> GetAllExperience()
        {
            return _context.Experiences.OrderByDescending(e => e.StartDate).ToList();
        }

        public Experience GetExperienceById(int id)
        {
            return _context.Experiences.FirstOrDefault(e => e.Id == id);
        }

        public bool AddExperience(Experience experience)
        {
            try
            {
                _context.Experiences.Add(experience);
                return SaveChanges();
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateExperience(Experience experience)
        {
            try
            {
                var existingExperience = _context.Experiences.Find(experience.Id);
                if (existingExperience == null)
                {
                    return false;
                }
                
                _context.Entry(existingExperience).CurrentValues.SetValues(experience);
                return SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating experience: {ex.Message}");
                return false;
            }
        }

        public bool DeleteExperience(int id)
        {
            try
            {
                var experience = _context.Experiences.Find(id);
                if (experience != null)
                {
                    _context.Experiences.Remove(experience);
                    return SaveChanges();
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Skills
        public IEnumerable<Skill> GetAllSkills()
        {
            return _context.Skills.OrderBy(s => s.Category).ThenBy(s => s.Name).ToList();
        }

        public Skill GetSkillById(int id)
        {
            return _context.Skills.FirstOrDefault(s => s.Id == id);
        }

        public bool AddSkill(Skill skill)
        {
            try
            {
                _context.Skills.Add(skill);
                return SaveChanges();
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateSkill(Skill skill)
        {
            try
            {
                var existingSkill = _context.Skills.Find(skill.Id);
                if (existingSkill == null)
                {
                    return false;
                }
                
                _context.Entry(existingSkill).CurrentValues.SetValues(skill);
                return SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating skill: {ex.Message}");
                return false;
            }
        }

        public bool DeleteSkill(int id)
        {
            try
            {
                var skill = _context.Skills.Find(id);
                if (skill != null)
                {
                    _context.Skills.Remove(skill);
                    return SaveChanges();
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Projects
        public IEnumerable<Project> GetAllProjects()
        {
            try
            {
                // Loglama ekle
                Console.WriteLine("Attempting to get all projects from database...");
                
                // SQL sorgusu hakkında detaylı bilgi
                var projects = _context.Projects.OrderByDescending(p => p.CompletionDate).ToList();
                
                Console.WriteLine($"Successfully retrieved {projects.Count} projects from database.");
                
                return projects;
            }
            catch (Exception ex)
            {
                // Detaylı hata loglaması
                Console.WriteLine($"ERROR in GetAllProjects: {ex.Message}");
                Console.WriteLine($"Exception type: {ex.GetType().Name}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // İç hata varsa onu da logla
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner exception type: {ex.InnerException.GetType().Name}");
                }
                
                return new List<Project>();
            }
        }

        public Project GetProjectById(int id)
        {
            return _context.Projects.FirstOrDefault(p => p.Id == id);
        }

        public string ProjectCount()
        {
            
            return _context.Projects.Count().ToString();
        }
        public bool AddProject(Project project)
        {
            try
            {
                _context.Projects.Add(project);
                return SaveChanges();
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateProject(Project project)
        {
            try
            {
                Console.WriteLine($"[UpdateProject] başlatıldı - ID: {project.Id}, Title: {project.Title}");
                
                var existingProject = _context.Projects.Find(project.Id);
                if (existingProject == null)
                {
                    Console.WriteLine($"[UpdateProject] HATA: Proje bulunamadı - ID: {project.Id}");
                    return false;
                }
                
                Console.WriteLine($"[UpdateProject] Mevcut proje bulundu - ID: {existingProject.Id}, Title: {existingProject.Title}");
                
                // Tracking durumunu kontrol et
                var entry = _context.Entry(existingProject);
                Console.WriteLine($"[UpdateProject] Entity state başlangıç: {entry.State}");
                
                // Her bir özelliği manuel olarak ayarla
                Console.WriteLine("[UpdateProject] Özellikleri güncelleme:");
                
                Console.WriteLine($" - Title: '{existingProject.Title}' -> '{project.Title}'");
                existingProject.Title = project.Title;
                
                Console.WriteLine($" - Description: '{existingProject.Description?.Substring(0, Math.Min(existingProject.Description?.Length ?? 0, 30))}...' -> '{project.Description?.Substring(0, Math.Min(project.Description?.Length ?? 0, 30))}...'");
                existingProject.Description = project.Description;
                
                Console.WriteLine($" - ImageUrl: '{existingProject.ImageUrl}' -> '{project.ImageUrl}'");
                existingProject.ImageUrl = project.ImageUrl;
                
                Console.WriteLine($" - ProjectUrl: '{existingProject.ProjectUrl}' -> '{project.ProjectUrl}'");
                existingProject.ProjectUrl = project.ProjectUrl;
                
                Console.WriteLine($" - Category: '{existingProject.Category}' -> '{project.Category}'");
                existingProject.Category = project.Category;
                
                Console.WriteLine($" - Client: '{existingProject.Client}' -> '{project.Client}'");
                existingProject.Client = project.Client;
                
                Console.WriteLine($" - CompletionDate: '{existingProject.CompletionDate}' -> '{project.CompletionDate}'");
                existingProject.CompletionDate = project.CompletionDate;
                
                // Entry durumunu kontrol et ve loglama yap
                Console.WriteLine($"[UpdateProject] Entity state değişikliklerden sonra: {entry.State}");
                
                // Entry'nin değişiklik durumlarını inceleme
                foreach (var property in entry.Properties)
                {
                    Console.WriteLine($" - Property: {property.Metadata.Name}, IsModified: {property.IsModified}");
                    if (property.IsModified)
                    {
                        Console.WriteLine($"   * Eski değer: {property.OriginalValue}, Yeni değer: {property.CurrentValue}");
                    }
                }
                
                // Durumu açıkça ayarla
                _context.Entry(existingProject).State = EntityState.Modified;
                Console.WriteLine($"[UpdateProject] Entity state açıkça Modified olarak ayarlandı: {entry.State}");
                
                // Değişiklikleri kaydet
                Console.WriteLine("[UpdateProject] SaveChanges çağrılıyor...");
                var result = SaveChanges();
                Console.WriteLine($"[UpdateProject] SaveChanges sonucu: {result}");
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateProject] HATA: {ex.Message}");
                Console.WriteLine($"[UpdateProject] Hata tipi: {ex.GetType().Name}");
                Console.WriteLine($"[UpdateProject] Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[UpdateProject] İç Hata: {ex.InnerException.Message}");
                    Console.WriteLine($"[UpdateProject] İç hata tipi: {ex.InnerException.GetType().Name}");
                }
                
                return false;
            }
        }

        public bool DeleteProject(int id)
        {
            try
            {
                var project = _context.Projects.Find(id);
                if (project != null)
                {
                    _context.Projects.Remove(project);
                    return SaveChanges();
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Services
        public IEnumerable<Service> GetAllServices()
        {
            return _context.Services.ToList();
        }

        public Service GetServiceById(int id)
        {
            return _context.Services.FirstOrDefault(s => s.Id == id);
        }

        public bool AddService(Service service)
        {
            try
            {
                Console.WriteLine($"Starting AddService: Title='{service.Title}', IconClass='{service.IconClass}', IconName='{service.IconName}', Order={service.Order}");
                Console.WriteLine($"Database state before add: {_context.Services.Count()} services in database");
                
                _context.Services.Add(service);
                Console.WriteLine("Service added to context, calling SaveChanges");
                
                var saved = SaveChanges();
                Console.WriteLine($"SaveChanges returned: {saved}");
                
                // Verify the service was actually saved
                var serviceInDb = _context.Services.FirstOrDefault(s => s.Title == service.Title && 
                                                                      (service.Id == 0 || s.Id == service.Id));
                
                if (serviceInDb != null)
                {
                    Console.WriteLine($"Service found in database after save, ID: {serviceInDb.Id}");
                }
                else
                {
                    Console.WriteLine("WARNING: Service not found in database after save attempt");
                }
                
                return saved;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding service: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        public bool UpdateService(Service service)
        {
            try
            {
                Console.WriteLine($"Updating service with ID: {service.Id}, Title: '{service.Title}'");
                var existingService = _context.Services.Find(service.Id);
                if (existingService == null)
                {
                    Console.WriteLine($"Service with ID {service.Id} not found in database");
                    return false;
                }
                
                Console.WriteLine("Found existing service, updating values...");
                _context.Entry(existingService).CurrentValues.SetValues(service);
                return SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating service: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        public bool DeleteService(int id)
        {
            try
            {
                var service = _context.Services.Find(id);
                if (service != null)
                {
                    _context.Services.Remove(service);
                    return SaveChanges();
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Contact Messages
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
            try
            {
                _context.ContactMessages.Add(message);
                return SaveChanges();
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteContactMessage(int id)
        {
            try
            {
                var message = _context.ContactMessages.Find(id);
                if (message != null)
                {
                    _context.ContactMessages.Remove(message);
                    return SaveChanges();
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // CV File
        public CVFile GetActiveCVFile()
        {
            return _context.CVFiles.FirstOrDefault(cf => cf.IsActive);
        }

        public IEnumerable<CVFile> GetAllCVFiles()
        {
            return _context.CVFiles.OrderByDescending(cf => cf.UploadDate).ToList();
        }

        public bool AddCVFile(CVFile cvFile)
        {
            try
            {
                // If this is set as active, deactivate all other CV files
                if (cvFile.IsActive)
                {
                    var existingActiveFiles = _context.CVFiles.Where(cf => cf.IsActive);
                    foreach (var file in existingActiveFiles)
                    {
                        file.IsActive = false;
                    }
                }

                cvFile.UploadDate = DateTime.Now;
                _context.CVFiles.Add(cvFile);
                return SaveChanges();
            }
            catch
            {
                return false;
            }
        }

        public bool SetCVFileActive(int id)
        {
            try
            {
                // Deactivate all CV files
                var allFiles = _context.CVFiles;
                foreach (var file in allFiles)
                {
                    file.IsActive = false;
                }

                // Set the selected one as active
                var cvFile = _context.CVFiles.Find(id);
                if (cvFile != null)
                {
                    cvFile.IsActive = true;
                    return SaveChanges();
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteCVFile(int id)
        {
            try
            {
                var cvFile = _context.CVFiles.Find(id);
                if (cvFile != null)
                {
                    _context.CVFiles.Remove(cvFile);
                    return SaveChanges();
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Medium Articles
        public IEnumerable<MediumArticle> GetAllMediumArticles()
        {
            return _context.MediumArticles.OrderByDescending(a => a.PublishedDate).ThenBy(a => a.Order).ToList();
        }

        public MediumArticle GetMediumArticleById(int id)
        {
            return _context.MediumArticles.FirstOrDefault(a => a.Id == id);
        }

        public bool AddMediumArticle(MediumArticle article)
        {
            try
            {
                _context.MediumArticles.Add(article);
                return SaveChanges();
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateMediumArticle(MediumArticle article)
        {
            try
            {
                var existingArticle = _context.MediumArticles.Find(article.Id);
                if (existingArticle == null)
                {
                    return false;
                }
                
                _context.Entry(existingArticle).CurrentValues.SetValues(article);
                return SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating medium article: {ex.Message}");
                return false;
            }
        }

        public bool DeleteMediumArticle(int id)
        {
            try
            {
                var article = _context.MediumArticles.Find(id);
                if (article != null)
                {
                    _context.MediumArticles.Remove(article);
                    return SaveChanges();
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Site Settings
        public SiteSettings GetSiteSettings()
        {
            return _context.SiteSettings.FirstOrDefault() ?? new SiteSettings();
        }

        public bool UpdateSiteSettings(SiteSettings settings)
        {
            try
            {
                var existingSettings = _context.SiteSettings.FirstOrDefault();
                if (existingSettings != null)
                {
                    _context.Entry(existingSettings).CurrentValues.SetValues(settings);
                }
                else
                {
                    _context.SiteSettings.Add(settings);
                }
                return SaveChanges();
            }
            catch
            {
                return false;
            }
        }

        // Save changes
        public bool SaveChanges()
        {
            try
            {
                Console.WriteLine("[SaveChanges] *** SaveChanges metodu başlatıldı ***");
                
                // Detaylı entity değişiklik izleme
                var entities = _context.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || 
                                e.State == EntityState.Modified || 
                                e.State == EntityState.Deleted)
                    .ToList();
                    
                Console.WriteLine($"[SaveChanges] İzlenen değişikliklerin sayısı: {entities.Count}");
                
                if (entities.Count == 0)
                {
                    Console.WriteLine("[SaveChanges] UYARI: Kaydedilecek değişiklik yok!");
                    return false;
                }
                
                // Her bir entity için detaylı değişiklik bilgisi
                foreach (var entry in entities)
                {
                    var entityName = entry.Entity.GetType().Name;
                    var entityId = entry.Property("Id")?.CurrentValue?.ToString() ?? "Unknown";
                    
                    Console.WriteLine($"[SaveChanges] Entity: {entityName}, ID: {entityId}, State: {entry.State}");
                    
                    if (entry.State == EntityState.Modified)
                    {
                        var modifiedProperties = entry.Properties
                            .Where(p => p.IsModified)
                            .ToList();
                        
                        if (modifiedProperties.Count == 0)
                        {
                            Console.WriteLine($"[SaveChanges] UYARI: {entityName} (ID:{entityId}) modified olarak işaretlendi ama değişmiş property yok!");
                        }
                        
                        foreach (var property in modifiedProperties)
                        {
                            var oldValue = property.OriginalValue;
                            var newValue = property.CurrentValue;
                            Console.WriteLine($"[SaveChanges]  - {property.Metadata.Name}: '{oldValue}' -> '{newValue}'");
                        }
                    }
                }

                try 
                {
                    // Kendi transaction'ımızı oluşturalım
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            Console.WriteLine("[SaveChanges] Database transaction başlatıldı");
                            Console.WriteLine("[SaveChanges] SaveChanges çağrılıyor...");
                            
                            // Asıl SaveChanges çağrısı
                            int result = _context.SaveChanges();
                            
                            // Etkilenen satır sayısını kontrol et
                            Console.WriteLine($"[SaveChanges] Etkilenen satır sayısı: {result}");
                            
                            if (result > 0)
                            {
                                // Transaction'ı onayla
                                transaction.Commit();
                                Console.WriteLine("[SaveChanges] Transaction başarıyla commit edildi");
                            }
                            else 
                            {
                                Console.WriteLine("[SaveChanges] UYARI: Değişiklikler kaydedildi ancak etkilenen satır sayısı 0");
                                // Yine de commit edelim
                                transaction.Commit();
                            }
                            
                            // Başarılı olduğunu belirt
                            return result > 0;
                        }
                        catch (Exception innerEx)
                        {
                            // Transaction'ı geri al
                            transaction.Rollback();
                            Console.WriteLine("[SaveChanges] Transaction rollback edildi");
                            
                            // Hatayı yukarı fırlat
                            throw;
                        }
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine("[SaveChanges] HATA - DbUpdateConcurrencyException: " + ex.Message);
                    Console.WriteLine("[SaveChanges] Bu hata genellikle birden fazla kullanıcı aynı veriyi aynı anda değiştirmeye çalıştığında oluşur");
                    
                    foreach (var entry in ex.Entries)
                    {
                        Console.WriteLine($"[SaveChanges] Etkilenen entity: {entry.Entity.GetType().Name}, State: {entry.State}");
                    }
                    
                    throw;
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine("[SaveChanges] HATA - DbUpdateException: " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine("[SaveChanges] İç Hata: " + ex.InnerException.Message);
                        Console.WriteLine($"[SaveChanges] İç Hata Tipi: {ex.InnerException.GetType().Name}");
                    }
                    
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SaveChanges] KRİTİK HATA: {ex.Message}");
                Console.WriteLine($"[SaveChanges] Hata tipi: {ex.GetType().Name}");
                
                // İç içe exception'ları logla
                var currentEx = ex;
                int exceptionDepth = 0;
                while (currentEx.InnerException != null)
                {
                    exceptionDepth++;
                    currentEx = currentEx.InnerException;
                    Console.WriteLine($"[SaveChanges] İç Hata {exceptionDepth}: {currentEx.Message}");
                    Console.WriteLine($"[SaveChanges] İç Hata {exceptionDepth} Tipi: {currentEx.GetType().Name}");
                }
                
                Console.WriteLine($"[SaveChanges] Stack Trace: {ex.StackTrace}");
                return false;
            }
        }
    }
} 