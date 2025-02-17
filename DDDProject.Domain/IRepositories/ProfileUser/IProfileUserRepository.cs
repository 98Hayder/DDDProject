using BookstoreAPI.Dtos.ProfileUserDto;
using BookstoreAPI.Dtos;

namespace DDDProject.Domain.IRepositories.ProfileUser
{
    public interface IProfileUserRepository
    {
     Task<MessageDto<ProfileUserForm>> GetUserProfileAsync(int userId);
     Task<MessageDto<ProfileUserForm>> UpdateUserProfileAsync(int userId, ProfileUserForm userForm);
    }
}
