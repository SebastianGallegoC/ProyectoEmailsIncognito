namespace Application.DTOs
{
    public class ContactResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsBlocked { get; set; }
    }
}
