namespace DDDProject.Domain.Dtos.BookDto
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string GenreName { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string? BookImage { get; set; }

    }
}
