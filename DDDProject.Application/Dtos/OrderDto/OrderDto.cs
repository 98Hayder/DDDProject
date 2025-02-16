using DDDProject.Domain.Enums;

namespace DDDProject.Domain.Dtos.OrderDto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } 
        public List<OrderDetailForm> OrderDetail { get; set; }
    }
}
