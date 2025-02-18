using DDDProject.Domain.Entities;
using DDDProject.Domain.ValueObjects;

namespace DDDProject.Domain.IRepositories.Book
{
    public interface IBookRepository
    {
        Task<IEnumerable<Domain.Entities.Book>> GetBooksAsync(BookFilter filter);
    }
}
