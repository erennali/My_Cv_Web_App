using ErenAliKocaCV.Models;
using Microsoft.EntityFrameworkCore;

namespace ErenAliKocaCV.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PersonalInfo> PersonalInfos { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<CVFile> CVFiles { get; set; }
        public DbSet<MediumArticle> MediumArticles { get; set; }
        public DbSet<SiteSettings> SiteSettings { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<FeaturedRepository> FeaturedRepositories { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Seed data for personal info
            modelBuilder.Entity<PersonalInfo>().HasData(
                new PersonalInfo
                {
                    Id = 1,
                    FullName = "Eren Ali Koca",
                    Title = "Software Developer",
                    Bio = "A passionate software developer with experience in building web applications.",
                    Email = "contact@erenalikoca.com",
                    Phone = "+90 XXX XXX XX XX",
                    Address = "Turkey",
                    Website = "erenalikoca.com",
                    ProfileImage = "/clark-master/images/bg_1.png",
                    BirthDate = new DateTime(1990, 1, 1) // Default date, replace with actual
                }
            );
            
            // Seed services
            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, Title = "Web Development", Description = "Building modern and responsive web applications.", IconName = "flaticon-analysis", IconClass = "fas fa-globe", Order = 1 },
                new Service { Id = 2, Title = "Mobile App Development", Description = "Creating native and cross-platform mobile applications.", IconName = "flaticon-flasks", IconClass = "fas fa-mobile-alt", Order = 2 },
                new Service { Id = 3, Title = "Database Design", Description = "Designing efficient and scalable database solutions.", IconName = "flaticon-ideas", IconClass = "fas fa-database", Order = 3 }
            );
            
            // Seed skills
            modelBuilder.Entity<Skill>().HasData(
                new Skill { Id = 1, Name = "C#", Proficiency = 90, Category = "Programming" },
                new Skill { Id = 2, Name = "ASP.NET Core", Proficiency = 85, Category = "Framework" },
                new Skill { Id = 3, Name = "JavaScript", Proficiency = 80, Category = "Programming" },
                new Skill { Id = 4, Name = "SQL", Proficiency = 90, Category = "Database" }
            );
            
            // Seed admin user
            modelBuilder.Entity<AdminUser>().HasData(
                new AdminUser
                {
                    Id = 1,
                    Username = "admin",
                    // This is a placeholder. In production, use proper password hashing
                    Password = "admin123", 
                    Email = "admin@example.com"
                }
            );
            
            // Index for ContactMessages for faster message queries
            modelBuilder.Entity<ContactMessage>()
                .HasIndex(m => m.DateSent)
                .HasDatabaseName("IX_ContactMessages_DateSent");
            
            modelBuilder.Entity<ContactMessage>()
                .HasIndex(m => m.IsRead)
                .HasDatabaseName("IX_ContactMessages_IsRead");
            
            // Index for ActivityLog for faster activity queries
            if (modelBuilder.Model.FindEntityType(typeof(ActivityLog)) != null)
            {
                modelBuilder.Entity<ActivityLog>()
                    .HasIndex(a => a.Timestamp)
                    .HasDatabaseName("IX_ActivityLog_Timestamp");
                
                modelBuilder.Entity<ActivityLog>()
                    .HasIndex(a => a.Action)
                    .HasDatabaseName("IX_ActivityLog_Action");
                
                modelBuilder.Entity<ActivityLog>()
                    .HasIndex(a => a.IsSuccess)
                    .HasDatabaseName("IX_ActivityLog_IsSuccess");
                
                // Composite index for login success status queries
                modelBuilder.Entity<ActivityLog>()
                    .HasIndex(a => new { a.Action, a.IsSuccess, a.Timestamp })
                    .HasDatabaseName("IX_ActivityLog_Action_IsSuccess_Timestamp");
            }
            
            // Add indexes for Skills and Projects to optimize category queries
            modelBuilder.Entity<Skill>()
                .HasIndex(s => s.Category)
                .HasDatabaseName("IX_Skills_Category");
            
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.Category)
                .HasDatabaseName("IX_Projects_Category");
            
            // Create indexes for UpdatedAt fields for faster last update queries
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var updatedAtProperty = entityType.FindProperty("UpdatedAt");
                if (updatedAtProperty != null)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasIndex(updatedAtProperty.Name)
                        .HasDatabaseName($"IX_{entityType.GetTableName()}_{updatedAtProperty.Name}");
                }
            }
        }
    }
} 