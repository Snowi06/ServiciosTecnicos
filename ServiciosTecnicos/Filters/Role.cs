namespace ServiciosTecnicos.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}