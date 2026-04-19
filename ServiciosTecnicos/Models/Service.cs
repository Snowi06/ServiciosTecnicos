namespace ServiciosTecnicos.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        public int RequestId { get; set; }
        public int TechnicianId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string FinalStatus { get; set; } = "completed";

        // Navigation properties
        public ServiceRequest Request { get; set; } = null!;
        public Technician Technician { get; set; } = null!;
        public Rating? Rating { get; set; }
    }
}