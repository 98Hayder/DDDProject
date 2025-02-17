using AutoMapper;
using BookstoreAPI.DbContexts;
using BookstoreAPI.Dtos;
using BookstoreAPI.Dtos.LogDto;
using Microsoft.EntityFrameworkCore;

namespace BookstoreAPI.Repositories.Log
{
    public class LogRepository: ILogRepository
    {
        private readonly ApplicationDbContext _context;

        public LogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResultDto<Entities.Logging>> GetLogAsync(LogFilterForm filter)
        {
            int currentPage = Math.Max(filter.PageNumber ?? 1, 1);
            int currentPageSize = Math.Max(filter.PageSize ?? 10, 1);
            var query = _context.Logs.AsQueryable();

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)currentPageSize);

            var log = await query
                     .OrderByDescending(c => c.Id)
                    .Skip((currentPage - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToListAsync();


            return new PaginatedResultDto<Entities.Logging>
            {
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageNumber = currentPage,
                PageSize = currentPageSize,
                Data = log,
            };

        }

    }
}
