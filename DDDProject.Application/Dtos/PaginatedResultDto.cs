namespace DDDProject.Domain.Dtos
{
    public class PaginatedResultDto<T>
    {
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
