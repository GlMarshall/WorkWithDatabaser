namespace Rnd.Models.DTOs
{
    public class CreateOrderDto
    {
        public decimal Amount { get; set; }
        public string? Desciption { get; set; }
        public int PersonId { get; set; }
    }
}
