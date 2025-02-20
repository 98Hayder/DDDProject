using AutoMapper;
using DDDProject.Domain.Dtos;
using DDDProject.Domain.Dtos.BookDto;
using DDDProject.Domain.IRepositories.Book;
using DDDProject.Domain.ValueObjects;


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
    }
}