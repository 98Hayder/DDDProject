using System.ComponentModel.DataAnnotations;

namespace DDDProject.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int GenreId { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string? BookImage { get; set; }
        public int Quantity { get; set; }

        public Genre Genre { get; set; }

    }
}
