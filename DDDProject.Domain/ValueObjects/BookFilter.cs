using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDProject.Domain.ValueObjects
{
    public class BookFilter
    {
        public string? SearchTerm { get; }
        public int? GenreId { get; }
        public bool? IsAvailable { get; }
        public decimal? MinPrice { get; }
        public decimal? MaxPrice { get; }
        public string SortBy { get; }
        public bool IsDescending { get; }
        public int PageNumber { get; }
        public int PageSize { get; }


        public BookFilter(string? searchTerm, int? genreId, bool? isAvailable, decimal? minPrice, decimal? maxPrice,
                  string sortBy , bool isDescending , int pageNumber , int pageSize)
        {
            SearchTerm = searchTerm;
            GenreId = genreId;
            IsAvailable = isAvailable;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            SortBy = sortBy;
            IsDescending = isDescending;
            PageNumber = pageNumber;
            PageSize = pageSize; 
        }
    }
}
