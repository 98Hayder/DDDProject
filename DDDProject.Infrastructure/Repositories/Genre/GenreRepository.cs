using AutoMapper;
using DDDProject.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using DDDProject.Domain.IRepositories.Genre;

namespace DDDProject.Infrastructure.Repositories.Genre
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GenreRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedResultDto<GenreDto>> GetGenreAsync(GenreFormFilter filter)
        {
            int currentPage = Math.Max(filter.PageNumber ?? 1, 1);
            int currentPageSize = Math.Max(filter.PageSize ?? 10, 1);
            var query = _context.Genres.AsQueryable();

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(c => c.Name.Contains(filter.SearchTerm));
            }

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)currentPageSize);

            var genres = await query
                     .OrderByDescending(c => c.Id)    
                    .Skip((currentPage - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToListAsync();

            var GenreDtos = _mapper.Map<List<GenreDto>>(genres);

            return new PaginatedResultDto<GenreDto>
            {
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageNumber = currentPage,
                PageSize = currentPageSize,
                Data = GenreDtos
            };

        }

        public async Task<bool> GenreExistsAsync(string name)
        {
            return await _context.Genres.AnyAsync(c => c.Name == name);
        }

        public async Task<MessageDto<GenreDto>> GetGenreByIdAsync(int id)
        {
            var Genre = await _context.Genres.FindAsync(id);

            if (Genre == null)
            {
                return new MessageDto<GenreDto>
                {
                    Success = false,
                    Message = "الفئة غير موجودة"
                };
            }

            var GenreDto = _mapper.Map<GenreDto>(Genre);

            return new MessageDto<GenreDto>
            {
                Success = true,
                Message = "الفئة موجودة",
                Data = GenreDto
            };
        }

        public async Task<MessageDto<GenreDto>> AddGenreAsync(GenreForm GenreForm)
        {
            if (await GenreExistsAsync(GenreForm.Name))
            {
                return new MessageDto<GenreDto>
                {
                    Success = false,
                    Message = "الفئة موجودة بالفعل"
                };
            }

            var Genre = _mapper.Map<Entities.Genre>(GenreForm);

            _context.Genres.Add(Genre);
            await _context.SaveChangesAsync();

            var GenreDto = _mapper.Map<GenreDto>(Genre);

            return new MessageDto<GenreDto>
            {
                Success = true,
                Message = "تم إضافة الفئة بنجاح",
                Data = GenreDto
            };
        }


        public async Task<MessageDto<GenreDto>> UpdateGenreAsync(int id, GenreForm GenreForm)
        {
            var Genre = await _context.Genres.FindAsync(id);

            if (Genre == null)
            {
                return new MessageDto<GenreDto>
                {
                    Success = false,
                    Message = "الفئة غير موجودة"
                };
            }

            if (!string.IsNullOrEmpty(GenreForm.Name) &&
                Genre.Name != GenreForm.Name &&
                await GenreExistsAsync(GenreForm.Name))
            {
                return new MessageDto<GenreDto>
                {
                    Success = false,
                    Message = "الفئة موجودة بالفعل"
                };
            }

            if (!string.IsNullOrEmpty(GenreForm.Name))
            {
                Genre.Name = GenreForm.Name;
            }


            _context.Genres.Update(Genre);
            await _context.SaveChangesAsync();

            var GenreDto = _mapper.Map<GenreDto>(Genre);


            return new MessageDto<GenreDto>
            {
                Success = true,
                Message = "تم تحديث الفئة بنجاح",
                Data = GenreDto
            };
        }

        public async Task<MessageDto<GenreDto>> DeleteGenreAsync(int id)
        {
            var Genre = await _context.Genres
                                         .Include(c => c.Books)
                                         .FirstOrDefaultAsync(c => c.Id == id);

            if (Genre == null)
            {
                return new MessageDto<GenreDto>
                {
                    Success = false,
                    Message = "الفئة غير موجودة"
                };
            }

            if (Genre.Books != null && Genre.Books.Any())
            {
                return new MessageDto<GenreDto>
                {
                    Success = false,
                    Message = $"لا يمكنك حذف هذه الفئة {Genre.Name} لأنها مرتبط بكتاب"
                };
            }

            _context.Genres.Remove(Genre);
            await _context.SaveChangesAsync();

            return new MessageDto<GenreDto>
            {
                Success = true,
                Message = "تم حذف الفئة بنجاح"
            };
        }
    }
}
