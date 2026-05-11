namespace ServiciosTecnicos.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int ServiceId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public Service Service { get; set; } = null!;
    }
}