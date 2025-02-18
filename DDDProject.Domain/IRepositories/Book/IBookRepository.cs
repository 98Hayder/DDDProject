using DDDProject.Domain.Entities;

namespace DDDProject.Domain.IRepositories.Book
{
    public interface IBookRepository
    {
        Task<List<Entities.Book>> GetBooksAsync(Entities.Book Book);
        Task<Entities.Book> GetBookByIdAsync(int id);
        Task AddBookAsync(Entities.Book book);
        Task<Entities.Book> UpdateBookAsync(int id, Entities.Book Book);
        Task<Entities.Book> DeleteBook(Entities.Book book);
    }
}
