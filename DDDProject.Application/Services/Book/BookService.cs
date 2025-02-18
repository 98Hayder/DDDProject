using DDDProject.Domain.Dtos;
using DDDProject.Domain.Dtos.BookDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDProject.Application.Services.Book
{
    public class BookService : IBookService
    {
        public async Task<MessageDto<BookDto>> AddBookAsync(BookForm bookForm)
        {
            throw new NotImplementedException();
        }

        public async Task<MessageDto<BookDto>> DeleteBookAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<MessageDto<BookDto>> GetBookByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginatedResultDto<BookDto>> GetBooksAsync(BookFilterForm filter)
        {
            throw new NotImplementedException();
        }

        public async Task<MessageDto<BookDto>> UpdateBookAsync(int id, BookForm bookForm)
        {
            throw new NotImplementedException();
        }
    }
}
