using DDDProject.Domain.Dtos.AuthDto;
using DDDProject.Domain.Dtos;

namespace DDDProject.Application.Services.Auth
{
    public interface IAuthService
    {
        MessageDto<LoginResponse> Login(LoginForm loginForm);
        MessageDto<string> Logout(string token);
        MessageDto<LoginResponse> Register(RegisterForm registerForm);

    }
}
