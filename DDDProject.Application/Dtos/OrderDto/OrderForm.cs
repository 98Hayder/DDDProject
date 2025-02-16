
namespace DDDProject.Domain.Dtos.OrderDto
{
    public class OrderForm
    {
        public int UserId { get; set; }
        public List<OrderDetailForm> OrderDetail { get; set; }
    }
}
