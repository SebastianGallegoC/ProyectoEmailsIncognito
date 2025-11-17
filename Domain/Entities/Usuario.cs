namespace Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = "User";
        public bool EstaActivo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
