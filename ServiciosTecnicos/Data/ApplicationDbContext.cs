using Microsoft.EntityFrameworkCore;
using ServiciosTecnicos.Models;

namespace ServiciosTecnicos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Technician> Technicians { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<TechnicianSpecialty> TechnicianSpecialties { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<RequestAssignment> RequestAssignments { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Role configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.RoleName).HasColumnName("role_name").HasMaxLength(30);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100);
                entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(150);
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
                entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(20);
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.RoleId);
            });

            // Client configuration
            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("clients");
                entity.HasKey(e => e.ClientId);
                entity.Property(e => e.ClientId).HasColumnName("client_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Address).HasColumnName("address").HasMaxLength(200);
                entity.Property(e => e.City).HasColumnName("city").HasMaxLength(100);

                entity.HasOne(e => e.User)
                    .WithOne(u => u.Client)
                    .HasForeignKey<Client>(e => e.UserId);
            });

            // Technician configuration
            modelBuilder.Entity<Technician>(entity =>
            {
                entity.ToTable("technicians");
                entity.HasKey(e => e.TechnicianId);
                entity.Property(e => e.TechnicianId).HasColumnName("technician_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.ExperienceYears).HasColumnName("experience_years");
                entity.Property(e => e.AverageRating).HasColumnName("average_rating").HasPrecision(3, 2);
                entity.Property(e => e.Available).HasColumnName("available");

                entity.HasOne(e => e.User)
                    .WithOne(u => u.Technician)
                    .HasForeignKey<Technician>(e => e.UserId);
            });

            // ServiceCategory configuration
            modelBuilder.Entity<ServiceCategory>(entity =>
            {
                entity.ToTable("service_categories");
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.CategoryName).HasColumnName("category_name").HasMaxLength(100);
                entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(200);
            });

            // Specialty configuration
            modelBuilder.Entity<Specialty>(entity =>
            {
                entity.ToTable("specialties");
                entity.HasKey(e => e.SpecialtyId);
                entity.Property(e => e.SpecialtyId).HasColumnName("specialty_id");
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.SpecialtyName).HasColumnName("specialty_name").HasMaxLength(100);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Specialties)
                    .HasForeignKey(e => e.CategoryId);
            });

            // TechnicianSpecialty configuration (many-to-many)
            modelBuilder.Entity<TechnicianSpecialty>(entity =>
            {
                entity.ToTable("technician_specialties");
                entity.HasKey(e => new { e.TechnicianId, e.SpecialtyId });
                entity.Property(e => e.TechnicianId).HasColumnName("technician_id");
                entity.Property(e => e.SpecialtyId).HasColumnName("specialty_id");

                entity.HasOne(e => e.Technician)
                    .WithMany(t => t.TechnicianSpecialties)
                    .HasForeignKey(e => e.TechnicianId);

                entity.HasOne(e => e.Specialty)
                    .WithMany(s => s.TechnicianSpecialties)
                    .HasForeignKey(e => e.SpecialtyId);
            });

            // ServiceRequest configuration
            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.ToTable("service_requests");
                entity.HasKey(e => e.RequestId);
                entity.Property(e => e.RequestId).HasColumnName("request_id");
                entity.Property(e => e.ClientId).HasColumnName("client_id");
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(150);
                entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
                entity.Property(e => e.Address).HasColumnName("address").HasMaxLength(200);
                entity.Property(e => e.RequestStatus).HasColumnName("request_status").HasMaxLength(30);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.HasOne(e => e.Client)
                    .WithMany(c => c.ServiceRequests)
                    .HasForeignKey(e => e.ClientId);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.ServiceRequests)
                    .HasForeignKey(e => e.CategoryId);
            });

            // RequestAssignment configuration
            modelBuilder.Entity<RequestAssignment>(entity =>
            {
                entity.ToTable("request_assignments");
                entity.HasKey(e => e.AssignmentId);
                entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
                entity.Property(e => e.RequestId).HasColumnName("request_id");
                entity.Property(e => e.TechnicianId).HasColumnName("technician_id");
                entity.Property(e => e.AssignedAt).HasColumnName("assigned_at");

                entity.HasOne(e => e.Request)
                    .WithOne(r => r.RequestAssignment)
                    .HasForeignKey<RequestAssignment>(e => e.RequestId);

                entity.HasOne(e => e.Technician)
                    .WithMany(t => t.RequestAssignments)
                    .HasForeignKey(e => e.TechnicianId);
            });

            // Service configuration
            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("services");
                entity.HasKey(e => e.ServiceId);
                entity.Property(e => e.ServiceId).HasColumnName("service_id");
                entity.Property(e => e.RequestId).HasColumnName("request_id");
                entity.Property(e => e.TechnicianId).HasColumnName("technician_id");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.EndDate).HasColumnName("end_date");
                entity.Property(e => e.FinalStatus).HasColumnName("final_status").HasMaxLength(30);

                entity.HasOne(e => e.Request)
                    .WithOne(r => r.Service)
                    .HasForeignKey<Service>(e => e.RequestId);

                entity.HasOne(e => e.Technician)
                    .WithMany(t => t.Services)
                    .HasForeignKey(e => e.TechnicianId);
            });

            // Rating configuration
            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("ratings");
                entity.HasKey(e => e.RatingId);
                entity.Property(e => e.RatingId).HasColumnName("rating_id");
                entity.Property(e => e.ServiceId).HasColumnName("service_id");
                entity.Property(e => e.Score).HasColumnName("score");
                entity.Property(e => e.Comment).HasColumnName("comment").HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.HasOne(e => e.Service)
                    .WithOne(s => s.Rating)
                    .HasForeignKey<Rating>(e => e.ServiceId);
            });
        }
    }
}