using BookstoreAPI.Dtos.LogDto;
using BookstoreAPI.Dtos;

namespace DDDProject.Domain.Repositories.Log
{
    public interface ILogRepository
    {
        Task<PaginatedResultDto<Entities.Logging>> GetLogAsync(LogFilterForm filter);
    }
}
