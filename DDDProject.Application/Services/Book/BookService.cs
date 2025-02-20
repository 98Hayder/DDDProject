using AutoMapper;
using DDDProject.Domain.Dtos;
using DDDProject.Domain.Dtos.BookDto;
using DDDProject.Domain.IRepositories.Book;
using DDDProject.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace DDDProject.Application.Services.Book
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BookService(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }


        public async Task<PaginatedResultDto<BookDto>> GetBooksAsync(BookFilterForm filterForm)
        {
            var filter = _mapper.Map<BookFilter>(filterForm);
            var books = await _bookRepository.GetBooksAsync(filter);
            var totalRecords = books.Count();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize);
            var bookDtos = _mapper.Map<List<BookDto>>(books);

            return new PaginatedResultDto<BookDto>
            {
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Data = bookDtos
            };
        }

        public async Task<MessageDto<BookDto>> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);

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
            if (await _bookRepository.ExistsAsync(bookForm.Title))
                return new MessageDto<BookDto> { Success = false, Message = "الكتاب موجود بالفعل" };

            if (await _bookRepository.GetGenreByIdAsync(bookForm.GenreId))
                return new MessageDto<BookDto> { Success = false, Message = "الفئة غير موجودة" };

            var imagePath = await SaveImageAsync(bookForm.BookImage);
            var book = _mapper.Map<Domain.Entities.Book>(bookForm);
            book.GenreId = bookForm.GenreId;
            book.BookImage = imagePath;
            var Addbook = await _bookRepository.AddBookAsync(book);
            var bookDto = _mapper.Map<BookDto>(Addbook);
            return new MessageDto<BookDto>
            {
                Success = true,
                Message = "تم إضافة الكتاب بنجاح",
                Data = bookDto
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