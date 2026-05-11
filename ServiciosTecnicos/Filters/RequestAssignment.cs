namespace ServiciosTecnicos.Models
{
    public class RequestAssignment
    {
        public int AssignmentId { get; set; }

        public int RequestId { get; set; }
        public ServiceRequest Request { get; set; } = null!;

        public int TechnicianId { get; set; }
        public Technician Technician { get; set; } = null!;

        public DateTime AssignedAt { get; set; }
    }
}

//Clase usada para asignar tecnico a solictudes