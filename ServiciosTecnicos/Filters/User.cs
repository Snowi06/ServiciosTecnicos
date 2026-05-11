namespace ServiciosTecnicos.Models
{
    public class User
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Role Role { get; set; } = null!;
        public Client? Client { get; set; }
        public Technician? Technician { get; set; }
    }
}