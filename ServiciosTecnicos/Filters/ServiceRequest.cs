namespace ServiciosTecnicos.Models
{
    public class ServiceRequest
    {
        public int RequestId { get; set; }
        public int ClientId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string RequestStatus { get; set; } = "pendiente";
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Client Client { get; set; } = null!;
        public ServiceCategory Category { get; set; } = null!;
        public RequestAssignment? RequestAssignment { get; set; }
        public Service? Service { get; set; }
    }
}