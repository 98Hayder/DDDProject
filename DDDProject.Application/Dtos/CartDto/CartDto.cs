namespace DDDProject.Domain.Dtos.CartDto
{
    public class CartDto
    {
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal TotalPrice => Items?.Sum(item => item.Price * item.Quantity) ?? 0;
    }
}
