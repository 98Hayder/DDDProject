using DDDProject.Domain.Entities;

namespace DDDProject.Domain.IRepositories.Auth
{
    public interface IAuthRepository
    {
        User? Login(string userName, string password);
        User Register(User newUser);
        bool Logout(string token);
        User? GetUserByUserName(string userName);

    }
}
