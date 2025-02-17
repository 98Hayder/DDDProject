using BookstoreAPI.Dtos.AuthDto;
using BookstoreAPI.Dtos;

namespace DDDProject.Domain.IRepositories.Login
{
    public interface IAuthRepository
    {
        MessageDto<LoginResponse> Login(LoginForm loginForm);

        MessageDto<LoginResponse> Register(RegisterForm registerForm);
        MessageDto<string> Logout(string token);

    }
}
