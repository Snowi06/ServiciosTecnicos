namespace ServiciosTecnicos.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}