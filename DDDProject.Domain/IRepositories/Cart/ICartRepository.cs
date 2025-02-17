using BookstoreAPI.Dtos.CartDto;
using BookstoreAPI.Dtos;

namespace DDDProject.Domain.Repositories.Cart
{
    public interface ICartRepository
    {
        Task<MessageDto<CartDto>> GetCartByUserIdAsync(int userId);
        Task<MessageDto<object>> AddToCartAsync(int userId, int bookId, int quantity);
        Task<MessageDto<object>> RemoveFromCartAsync(int userId, int bookId);
        Task<MessageDto<object>> ClearCartAsync(int userId);
        Task<MessageDto<int>> GetCartItemCountAsync(int userId);
        Task<MessageDto<decimal>> GetCartTotalAsync(int userId);
    }
}
