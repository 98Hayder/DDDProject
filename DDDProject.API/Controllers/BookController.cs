using DDDProject.Application.Services.Book;
using DDDProject.Domain.Dtos.BookDto;
using Microsoft.AspNetCore.Mvc;

namespace DDDProject.API.Controllers
{

    public class BookController : BaseController
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] BookFilterForm filterForm)
        {
            var books = await _bookService.GetBooksAsync(filterForm);
            return Ok(books);
        }

    }
}
