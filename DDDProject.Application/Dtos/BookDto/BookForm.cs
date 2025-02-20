using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DDDProject.Domain.Dtos.BookDto
{
    public class BookForm
    {
        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(200, ErrorMessage = "يجب ألا يتجاوز العنوان 200 حرف.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "المؤلف مطلوب")]
        [StringLength(150, ErrorMessage = "يجب ألا يتجاوز اسم المؤلف 150 حرف.")]
        public string Author { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "رقم التصنيف يجب أن يكون أكبر من 0.")]


        [Required(ErrorMessage = "الصنف مطلوب")]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0.1, 10000, ErrorMessage = "يجب أن يكون السعر بين 0.1 و 10000.")]
        public decimal Price { get; set; }


        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "رقم الكيمة يجب أن يكون أكبر من 0.")]

        public int AvailableQuantity { get; set; 
        }
        public bool IsAvailable { get; set; } = true;

        [Required(ErrorMessage = "الصورة مطلوبة")]
        public IFormFile? BookImage { get; set; }

    }
}
