using BookstoreAPI.Dtos.OrderDto;
using BookstoreAPI.Dtos;
using BookstoreAPI.Enums;

namespace DDDProject.Domain.IRepositories.Order
{
    public interface IOrderRepository
    {
        Task<MessageDto<OrderDto>> CreateOrderAsync(OrderForm dto);
        Task<MessageDto<OrderDto>> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        Task<MessageDto<List<OrderDto>>> GetOrdersByUserIdAsync(int userId);
        Task<MessageDto<List<OrderDto>>> GetAllOrdersAsync(OrderFilter filter);
    }
}
