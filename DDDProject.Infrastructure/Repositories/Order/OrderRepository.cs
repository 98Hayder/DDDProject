using AutoMapper;
using DDDProject.Infrastructure.DbContexts;
using BookstoreAPI.Dtos;
using BookstoreAPI.Dtos.OrderDto;
using BookstoreAPI.Entities;
using BookstoreAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace DDDProject.Infrastructure.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;


        public OrderRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<MessageDto<OrderDto>> CreateOrderAsync(OrderForm dto)
        {
            var customer = await _context.Users.FindAsync(dto.UserId);
            if (customer == null)
            {
                return new MessageDto<OrderDto>
                {
                    Success = false,
                    Message = "الزبون غير موجود.",
                    Data = null
                };
            }


            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var order = new Entities.Order
                    {
                        UserId = dto.UserId,
                        CreatedAt = DateTime.UtcNow,
                        TotalAmount = 0,
                        Status = OrderStatus.Pending,
                        OrderDetails = new List<OrderDetail>()
                    };

                    decimal totalAmount = 0;

                    foreach (var detailDto in dto.OrderDetail)
                    {
                        var book = await _context.Books.FindAsync(detailDto.BookId);
                        if (book == null)
                        {
                            return new MessageDto<OrderDto>
                            {
                                Success = false,
                                Message = $"الكتاب ذو المعرف {detailDto.BookId} غير موجود.",
                                Data = null
                            };
                        }


                        if (book.IsAvailable == false)
                        {
                            return new MessageDto<OrderDto>
                            {
                                Success = false,
                                Message = $"   الكتاب غير متوفر {book.Title}.",
                                Data = null
                            };
                        }


                        decimal itemTotal = book.Price * detailDto.Quantity;
                        totalAmount += itemTotal;

                        order.OrderDetails.Add(new OrderDetail
                        {
                            BookId = book.Id,
                            Quantity = detailDto.Quantity,
                            Price = book.Price,
                            TotalPrice = itemTotal
                        });
                    }

                    order.TotalAmount = totalAmount;

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    var orderDto = new OrderDto
                    {
                        Id = order.Id,
                        UserId = order.UserId,
                        OrderDate = order.CreatedAt,
                        TotalAmount = order.TotalAmount,
                        OrderDetail = order.OrderDetails.Select(d => new OrderDetailForm
                        {
                            BookId = d.BookId,
                            Quantity = d.Quantity,
                            UnitPrice = d.Price
                        }).ToList()
                    };

                    return new MessageDto<OrderDto>
                    {
                        Success = true,
                        Message = "تم إنشاء الطلب بنجاح.",
                        Data = orderDto
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new MessageDto<OrderDto>
                    {
                        Success = false,
                        Message = $"حدث خطأ أثناء إنشاء الطلب: {ex.Message}",
                        Data = null
                    };
                }
            }
        }

        public async Task<MessageDto<OrderDto>> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders
                .Include(x => x.OrderDetails)
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
            {
                return new MessageDto<OrderDto>
                {
                    Success = false,
                    Message = "الطلب غير موجود.",
                    Data = null
                };
            }

            order.Status = newStatus ;
            await _context.SaveChangesAsync();

            var orderDto = new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(), 
                OrderDetail = order.OrderDetails.Select(d => new OrderDetailForm
                {
                    BookId = d.BookId,
                    Quantity = d.Quantity,
                    UnitPrice = d.Price
                }).ToList()
            };

            return new MessageDto<OrderDto>
            {
                Success = true,
                Message = "تم تحديث حالة الطلب بنجاح.",
                Data = orderDto
            };
        }

        public async Task<MessageDto<List<OrderDto>>> GetOrdersByUserIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new MessageDto<List<OrderDto>>
                {
                    Success = false,
                    Message = "المستخدم غير موجود.",
                    Data = null
                };
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var orderDtos = orders.Select(order => new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                OrderDetail = order.OrderDetails.Select(d => new OrderDetailForm
                {
                    BookId = d.BookId,
                    Quantity = d.Quantity,
                    UnitPrice = d.Price
                }).ToList()
            }).ToList();

            return new MessageDto<List<OrderDto>>
            {
                Success = true,
                Message = "تم جلب الطلبات بنجاح.",
                Data = orderDtos
            };
        }


        public async Task<MessageDto<List<OrderDto>>> GetAllOrdersAsync(OrderFilter filter)
        {
            int currentPage = Math.Max(filter.PageNumber ?? 1, 1);
            int currentPageSize = Math.Max(filter.PageSize ?? 10, 1);
            var query = _context.Orders.Include(o => o.OrderDetails).AsQueryable();
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)currentPageSize);

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                    .Skip((currentPage - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToListAsync();

            var orderDtos = orders.Select(order => new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                OrderDetail = order.OrderDetails.Select(d => new OrderDetailForm
                {
                    BookId = d.BookId,
                    Quantity = d.Quantity,
                    UnitPrice = d.Price
                }).ToList()
            }).ToList();

            return new MessageDto<List<OrderDto>>
            {
                Success = true,
                Message = "تم جلب جميع الطلبات بنجاح.",
                Data = orderDtos
            };
        }




    }
}
