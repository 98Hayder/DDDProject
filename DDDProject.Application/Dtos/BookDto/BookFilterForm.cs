using System.ComponentModel.DataAnnotations;

namespace DDDProject.Domain.Dtos.BookDto
{
    public class BookFilterForm : Filter
    {

        [Range(1, int.MaxValue, ErrorMessage = "رقم التصنيف يجب أن يكون أكبر من 0.")]
        public int? GenreId { get; set; }
        public bool? IsAvailable { get; set; }

        [Range(0.1, 10000, ErrorMessage = "يجب أن يكون السعر الأدنى بين 0.1 و 10000.")]
        public decimal? MinPrice { get; set; }

        [Range(0.1, 10000, ErrorMessage = "يجب أن يكون السعر الأقصى بين 0.1 و 10000.")]
        public decimal? MaxPrice { get; set; }

        public string SortBy { get; set; } = "Id";
        public bool IsDescending { get; set; } = true;
    }
}
