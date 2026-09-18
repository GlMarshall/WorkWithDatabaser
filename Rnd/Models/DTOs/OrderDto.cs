namespace Rnd.Models.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public int PersonalId { get; set; }
        public string PersonFullName { get; set; } = string.Empty;
    }
}
