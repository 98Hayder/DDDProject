using BookstoreAPI.Dtos.ProfileUserDto;
using BookstoreAPI.Dtos;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using DDDProject.Infrastructure.DbContexts;

namespace DDDProject.Infrastructure.Repositories.ProfileUser
{
    public class ProfileUserRepository : IProfileUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;


        public ProfileUserRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<MessageDto<ProfileUserForm>> GetUserProfileAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return new MessageDto<ProfileUserForm>
                {
                    Success = false,
                    Message = "المستخدم غير موجود.",
                    Data = null
                };
            }

            var userDto = new ProfileUserForm
            {
                FullName = user.FullName,
                UserName = user.UserName,
            };

            return new MessageDto<ProfileUserForm>
            {
                Success = true,
                Message = "تم جلب بيانات المستخدم بنجاح.",
                Data = userDto
            };
        }

        public async Task<MessageDto<ProfileUserForm>> UpdateUserProfileAsync(int userId, ProfileUserForm userForm)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return new MessageDto<ProfileUserForm>
                {
                    Success = false,
                    Message = "المستخدم غير موجود.",
                    Data = null
                };
            }

            user.FullName = userForm.FullName;
            user.UserName = userForm.UserName;
            if (!string.IsNullOrWhiteSpace(userForm.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(userForm.Password);
            }

            await _context.SaveChangesAsync();
                return new MessageDto<ProfileUserForm>
                {
                    Success = true,
                    Message = "تم تحديث بيانات المستخدم بنجاح.",
                    Data = userForm
                };
        }
    }
}
