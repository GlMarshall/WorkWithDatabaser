namespace Rnd.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;
    }
}
