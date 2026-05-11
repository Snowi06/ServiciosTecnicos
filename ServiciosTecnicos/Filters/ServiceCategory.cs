namespace ServiciosTecnicos.Models
{
    public class ServiceCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Navigation properties
        public ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}