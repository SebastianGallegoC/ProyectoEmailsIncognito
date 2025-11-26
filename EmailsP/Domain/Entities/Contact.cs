namespace Domain.Entities
{
    public class Contact
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; } // NUEVO: Relación con usuario
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
        public string? Notes { get; set; }

        public bool IsFavorite { get; set; }
        public bool IsBlocked { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
