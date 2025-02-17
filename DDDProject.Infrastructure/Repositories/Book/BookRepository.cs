using AutoMapper;
using DDDProject.Infrastructure.DbContexts;
using BookstoreAPI.Dtos;
using BookstoreAPI.Dtos.BookDto;
using BookstoreAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DDDProject.Infrastructure.Repositories.Book
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BookRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedResultDto<BookDto>> GetBooksAsync(BookFilterForm filter)
        {
            int currentPage = Math.Max(filter.PageNumber ?? 1, 1);
            int currentPageSize = Math.Max(filter.PageSize ?? 10, 1);

            var query = _context.Books
                 .AsNoTracking()
                 .Include(b => b.Genre)
                 .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                string pattern = $"%{filter.SearchTerm}%";
                query = query.Where(c => EF.Functions.Like(c.Author, pattern) ||
                                         EF.Functions.Like(c.Title, pattern));
            }

            if (filter.GenreId.HasValue)
            {
                query = query.Where(b => b.GenreId == filter.GenreId.Value);
            }

            if (filter.IsAvailable.HasValue)
            {
                query = query.Where(c => c.IsAvailable == filter.IsAvailable.Value);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(c => c.Price >= filter.MinPrice.Value);
            }
            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= filter.MaxPrice.Value);
            }

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)currentPageSize);

            query = filter.SortBy.ToLower() switch
            {
                "title" => filter.IsDescending ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title),
                "author" => filter.IsDescending ? query.OrderByDescending(c => c.Author) : query.OrderBy(c => c.Author),
                "genre" => filter.IsDescending ? query.OrderByDescending(b => b.Genre.Name) : query.OrderBy(b => b.Genre.Name),
                "price" => filter.IsDescending ? query.OrderByDescending(c => c.Price) : query.OrderBy(c => c.Price),
                _ => filter.IsDescending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id)
            };

            var books = await query
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToListAsync();

            var bookDtos = _mapper.Map<List<BookDto>>(books);

            return new PaginatedResultDto<BookDto>
            {
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageNumber = currentPage,
                PageSize = currentPageSize,
                Data = bookDtos
            };
        }

        public async Task<MessageDto<BookDto>> GetBookByIdAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return new MessageDto<BookDto>
                {
                    Success = false,
                    Message = "الكتاب غير موجود"
                };
            }

            var bookDto = _mapper.Map<BookDto>(book);

            return new MessageDto<BookDto>
            {
                Success = true,
                Message = "الكتاب موجود",
                Data = bookDto
            };
        }

        public async Task<MessageDto<BookDto>> AddBookAsync(BookForm bookForm)
        {
            if (await _context.Books.AnyAsync(c => c.Title == bookForm.Title))
            {
                return new MessageDto<BookDto>
                {
                    Success = false,
                    Message = "الكتاب موجود بالفعل"
                };
            }

            var genre = await _context.Genres.FindAsync(bookForm.GenreId);
            if (genre == null)
            {
                return new MessageDto<BookDto>
                {
                    Success = false,
                    Message = "الفئة غير موجودة"
                };
            }

            var imagePath = await SaveImageAsync(bookForm.BookImage);

            var book = _mapper.Map<Entities.Book>(bookForm);
            book.Genre = genre;
            book.BookImage = imagePath;

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var bookDto = _mapper.Map<BookDto>(book);

            return new MessageDto<BookDto>
            {
                Success = true,
                Message = "تم إضافة الكتاب بنجاح",
                Data = bookDto
            };
        }




        public async Task<MessageDto<BookDto>> UpdateBookAsync(int id, BookForm bookForm)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return new MessageDto<BookDto>
                {
                    Success = false,
                    Message = "الكتاب غير موجود"
                };
            }

            book.Title = bookForm.Title ?? book.Title;
            book.Author = bookForm.Author ?? book.Author;
            book.GenreId = bookForm.GenreId ?? book.GenreId;
            book.Price = bookForm.Price > 0 ? bookForm.Price : book.Price;
            book.IsAvailable = bookForm.IsAvailable;

            if (bookForm.BookImage != null && bookForm.BookImage.Length > 0)
            {
                if (!string.IsNullOrEmpty(book.BookImage))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.BookImage);
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                var newImagePath = await SaveImageAsync(bookForm.BookImage);
                book.BookImage = newImagePath;
            }


            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            var bookDto = _mapper.Map<BookDto>(book);

            return new MessageDto<BookDto>
            {
                Success = true,
                Message = "تم تحديث الكتاب بنجاح",
                Data = bookDto
            };
        }

        public async Task<MessageDto<BookDto>> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return new MessageDto<BookDto>
                {
                    Success = false,
                    Message = "الكتاب غير موجود"
                };
            }

            if (!string.IsNullOrEmpty(book.BookImage))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.BookImage);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return new MessageDto<BookDto>
            {
                Success = true,
                Message = "تم حذف الكتاب بنجاح"
            };
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null;
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "BookImage");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return Path.Combine("BookImage", uniqueFileName);
        }

    }
}
