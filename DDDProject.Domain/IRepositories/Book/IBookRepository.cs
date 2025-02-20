using DDDProject.Domain.Entities;
using DDDProject.Domain.ValueObjects;

namespace DDDProject.Domain.IRepositories.Book
{
    public interface IBookRepository
    {
        Task<IEnumerable<Entities.Book>> GetBooksAsync(BookFilter filter);
        Task<Entities.Book> GetBookByIdAsync(int id);
        Task<bool> ExistsAsync(string title);
        Task<bool> GetGenreByIdAsync(int id);
        Task<Entities.Book> AddBookAsync(Entities.Book book);

    }
}
