//using AutoMapper;
//using DDDProject.Infrastructure.DbContexts;
//using Microsoft.EntityFrameworkCore;
//using DDDProject.Domain.IRepositories.Cart;

//namespace DDDProject.Infrastructure.Repositories.Cart
//{
//    public class CartRepository : ICartRepository
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IMapper _mapper;

//        public CartRepository(ApplicationDbContext context, IMapper mapper)
//        {
//            _context = context;
//            _mapper = mapper;
//        }


//        public async Task<MessageDto<CartDto>> GetCartByUserIdAsync(int userId)
//        {
//            var cart = await _context.Carts
//                .Include(c => c.Items)
//                .FirstOrDefaultAsync(c => c.UserId == userId);

//            if (cart == null)
//                return new MessageDto<CartDto> { Success = false, Message = "السلة غير موجودة" };

//            var cartDto = new CartDto
//            {
//                UserId = cart.UserId,
//                Items = cart.Items.Select(ci => new CartItemDto
//                {
//                    BookId = ci.BookId,
//                    Quantity = ci.Quantity,
//                    Price = ci.Price
//                }).ToList()
//            };

//            return new MessageDto<CartDto> { Success = true, Message = "تم جلب بيانات السلة", Data = cartDto };
//        }


//        public async Task<MessageDto<object>> AddToCartAsync(int userId, int bookId, int quantity)
//        {
//            var cart = await _context.Carts
//                .Include(c => c.Items)
//                .FirstOrDefaultAsync(c => c.UserId == userId);

//            if (cart == null)
//            {
//                cart = new Entities.Cart { UserId = userId };
//                _context.Carts.Add(cart);
//                await _context.SaveChangesAsync();
//            }

//            var book = await _context.Books.FindAsync(bookId);
//            if (book == null)
//                return new MessageDto<object> { Success = false, Message = "الكتاب غير موجود" };

//            var existingItem = cart.Items.FirstOrDefault(i => i.BookId == bookId);
//            if (existingItem != null)
//            {
//                existingItem.Quantity += quantity;
//            }
//            else
//            {
//                cart.Items.Add(new CartItem
//                {
//                    BookId = bookId,
//                    Quantity = quantity,
//                    Price = book.Price
//                });
//            }

//            await _context.SaveChangesAsync();
//            return new MessageDto<object> { Success = true, Message = "تمت إضافة الكتاب إلى السلة" };
//        }


//        public async Task<MessageDto<object>> RemoveFromCartAsync(int userId, int bookId)
//        {
//            var cart = await _context.Carts
//                .Include(c => c.Items)
//                .FirstOrDefaultAsync(c => c.UserId == userId);

//            if (cart == null)
//                return new MessageDto<object> { Success = false, Message = "السلة غير موجودة" };

//            var item = cart.Items.FirstOrDefault(i => i.BookId == bookId);
//            if (item != null)
//            {
//                cart.Items.Remove(item);
//                await _context.SaveChangesAsync();
//                return new MessageDto<object> { Success = true, Message = "تمت إزالة الكتاب من السلة" };
//            }

//            return new MessageDto<object> { Success = false, Message = "العنصر غير موجود في السلة" };
//        }


//        public async Task<MessageDto<object>> ClearCartAsync(int userId)
//        {
//            var cart = await _context.Carts
//                .Include(c => c.Items)
//                .FirstOrDefaultAsync(c => c.UserId == userId);

//            if (cart != null)
//            {
//                cart.Items.Clear();
//                await _context.SaveChangesAsync();
//                return new MessageDto<object> { Success = true, Message = "تم تفريغ السلة" };
//            }

//            return new MessageDto<object> { Success = false, Message = "السلة غير موجودة" };
//        }

//        public async Task<MessageDto<int>> GetCartItemCountAsync(int userId)
//        {
//            var count = await _context.CartItems
//                .Where(ci => ci.Cart.UserId == userId)
//                .SumAsync(ci => ci.Quantity);

//            return new MessageDto<int> { Success = true, Message = "تم جلب عدد العناصر في السلة", Data = count };
//        }


//        public async Task<MessageDto<decimal>> GetCartTotalAsync(int userId)
//        {
//            var total = await _context.CartItems
//                .Where(ci => ci.Cart.UserId == userId)
//                .SumAsync(ci => ci.Quantity * ci.Price);
//            return new MessageDto<decimal> { Success = true, Message = "تم حساب إجمالي السلة", Data = total };
//        }
//    }
//}
