namespace ServiciosTecnicos.Models
{
    public class Specialty
    {
        public int SpecialtyId { get; set; }
        public int CategoryId { get; set; }
        public string SpecialtyName { get; set; } = string.Empty;

        // Navigation properties
        public ServiceCategory Category { get; set; } = null!;
        public ICollection<TechnicianSpecialty> TechnicianSpecialties { get; set; } = new List<TechnicianSpecialty>();
    }
}