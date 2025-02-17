using BookstoreAPI.Dtos.LogDto;
using BookstoreAPI.Dtos;

namespace BookstoreAPI.Repositories.Log
{
    public interface ILogRepository
    {
        Task<PaginatedResultDto<Entities.Logging>> GetLogAsync(LogFilterForm filter);
    }
}
