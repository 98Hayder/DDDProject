namespace DDDProject.Domain.Dtos.CartDto
{
    public class CartItemDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
