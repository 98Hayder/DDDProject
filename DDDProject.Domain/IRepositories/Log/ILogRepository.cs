using BookstoreAPI.Dtos.LogDto;
using BookstoreAPI.Dtos;

namespace DDDProject.Domain.IRepositories.Log
{
    public interface ILogRepository
    {
        Task<PaginatedResultDto<Entities.Logging>> GetLogAsync(LogFilterForm filter);
    }
}
