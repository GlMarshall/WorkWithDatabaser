namespace Rnd.Models.DTOs
{
    public class UpdateOrderDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
