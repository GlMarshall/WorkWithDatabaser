namespace Rnd.Models.DTOs
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
    }
}
