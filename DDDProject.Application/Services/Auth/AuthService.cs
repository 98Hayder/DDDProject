using DDDProject.Domain.Dtos.AuthDto;
using DDDProject.Domain.Dtos;
using DDDProject.Domain.IRepositories.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DDDProject.Domain.Entities;

namespace DDDProject.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        public MessageDto<LoginResponse> Login(LoginForm loginForm)
        {
            var user = _authRepository.Login(loginForm.UserName, loginForm.Password);

            if (user == null)
            {
                return new MessageDto<LoginResponse>
                {
                    Success = false,
                    Message = "المستخدم غير موجود أو كلمة المرور غير صحيحة"
                };
            }

            var token = GenerateJwtToken(user.UserName, user.UserRole.Role, user.Id);

            return new MessageDto<LoginResponse>
            {
                Success = true,
                Data = new LoginResponse
                {
                    Token = token,
                    RoleName = user.UserRole.Role,
                }
            };
        }

        public MessageDto<string> Logout(string token)
        {
            bool isLoggedOut = _authRepository.Logout(token);

            if (!isLoggedOut)
            {
                return new MessageDto<string>
                {
                    Success = false,
                    Message = "التوكن غير صالح أو تم تسجيل الخروج مسبقًا"
                };
            }

            return new MessageDto<string>
            {
                Success = true,
                Message = "تم تسجيل الخروج بنجاح"
            };
        }

        public MessageDto<LoginResponse> Register(RegisterForm registerForm)
        {
            var existingUser = _authRepository.GetUserByUserName(registerForm.UserName);
            if (existingUser != null)
            {
                return new MessageDto<LoginResponse>
                {
                    Success = false,
                    Message = "اسم المستخدم مستخدم بالفعل"
                };
            }

            var newUser = new User
            {
                FullName = registerForm.FullName,
                UserName = registerForm.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(registerForm.Password),
                RoleID = 2
            };
            var createdUser = _authRepository.Register(newUser);

            if (createdUser.UserRole == null)
            {
                return new MessageDto<LoginResponse>
                {
                    Success = false,
                    Message = "فشل في استرداد دور المستخدم"
                };
            }

            // إنشاء التوكن JWT
            var token = GenerateJwtToken(createdUser.UserName, createdUser.UserRole.Role, createdUser.Id);
            return new MessageDto<LoginResponse>
            {
                Success = true,
                Message = "تم التسجيل بنجاح",
                Data = new LoginResponse
                {
                    Token = token,
                    RoleName = createdUser.UserRole.Role,
                }
            };
        }

        private string GenerateJwtToken(string userName, string roleName, int Id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim(ClaimTypes.NameIdentifier, Id.ToString()) 

                }),
                Expires = DateTime.UtcNow.AddHours(10),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
