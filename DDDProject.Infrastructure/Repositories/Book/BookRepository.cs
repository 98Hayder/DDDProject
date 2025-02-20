using AutoMapper;
using DDDProject.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using DDDProject.Domain.IRepositories.Book;
using DDDProject.Domain.ValueObjects;
using System.Linq;
using DDDProject.Domain.Entities;

namespace DDDProject.Infrastructure.Repositories.Book
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Book> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<IEnumerable<Domain.Entities.Book>> GetBooksAsync(BookFilter filter)
        {
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


            query = filter.SortBy.ToLower() switch
            {
                "title" => filter.IsDescending ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title),
                "author" => filter.IsDescending ? query.OrderByDescending(c => c.Author) : query.OrderBy(c => c.Author),
                "genre" => filter.IsDescending ? query.OrderByDescending(b => b.Genre.Name) : query.OrderBy(b => b.Genre.Name),
                "price" => filter.IsDescending ? query.OrderByDescending(c => c.Price) : query.OrderBy(c => c.Price),
                _ => filter.IsDescending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id)
            };
            return await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }



        public async Task<bool> ExistsAsync(string title)
        {
            return await _context.Books.AnyAsync(b => b.Title == title);
        }

        public async Task<bool> GetGenreByIdAsync(int genreId)
        {
            var genre = await _context.Genres.FindAsync(genreId);
            return genre != null;
        }


    }
}
