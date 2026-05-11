namespace ServiciosTecnicos.Models
{
    public class Solicitud
    {
        public int Id { get; set; } 
        public string NombreCliente { get; set; }
        public string TipoServicio { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha{ get; set; }
        public string Estado { get; set; }
    }
}
