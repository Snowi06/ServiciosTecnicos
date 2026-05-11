namespace ServiciosTecnicos.Models
{
    public class Technician
    {
        public int TechnicianId { get; set; }
        public int UserId { get; set; }
        public int ExperienceYears { get; set; }
        public decimal AverageRating { get; set; }
        public bool Available { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<TechnicianSpecialty> TechnicianSpecialties { get; set; } = new List<TechnicianSpecialty>();
        public ICollection<RequestAssignment> RequestAssignments { get; set; } = new List<RequestAssignment>();
        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}