﻿using DDDProject.Domain.Dtos.BookDto;
using DDDProject.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDProject.Application.Services.Book
{
    public interface IBookService
    {
     Task<PaginatedResultDto<BookDto>> GetBooksAsync(BookFilterForm filterForm);
     Task<MessageDto<BookDto>> GetBookByIdAsync(int id);
     Task<MessageDto<BookDto>> AddBookAsync(BookForm bookForm);

    }
}
