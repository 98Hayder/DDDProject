using BookstoreAPI.Dtos.BookDto;
using BookstoreAPI.Dtos;

namespace DDDProject.Domain.IRepositories.Book
{
    public interface IBookRepository
    {
        Task<PaginatedResultDto<BookDto>> GetBooksAsync(BookFilterForm filter);
        Task<MessageDto<BookDto>> GetBookByIdAsync(int id);
        Task<MessageDto<BookDto>> AddBookAsync(BookForm bookForm);
        Task<MessageDto<BookDto>> UpdateBookAsync(int id, BookForm bookForm);
        Task<MessageDto<BookDto>> DeleteBookAsync(int id);
    }
}
