namespace ServiciosTecnicos.Models
{
    public class TechnicianSpecialty
    {
        public int TechnicianId { get; set; }
        public int SpecialtyId { get; set; }

        // Navigation properties
        public Technician Technician { get; set; } = null!;
        public Specialty Specialty { get; set; } = null!;
    }
}