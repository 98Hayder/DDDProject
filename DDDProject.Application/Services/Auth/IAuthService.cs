using DDDProject.Domain.Dtos.AuthDto;
using DDDProject.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDProject.Application.Services.Auth
{
    public interface IAuthService
    {
        MessageDto<LoginResponse> Login(LoginForm loginForm);
        MessageDto<string> Logout(string token);
        MessageDto<LoginResponse> Register(RegisterForm registerForm);

    }
}
