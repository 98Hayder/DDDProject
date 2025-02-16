namespace DDDProject.Domain.Dtos.OrderDto
{
    public class OrderDetailForm
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

    }
}
