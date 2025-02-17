using DDDProject.Infrastructure.DbContexts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DDDProject.Domain.IRepositories.Login;

namespace DDDProject.Infrastructure.Repositories.Login
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public MessageDto<LoginResponse> Login(LoginForm loginForm)
        {

            var loginUser = _context.Users
                .AsNoTracking()
                .Where(u => u.UserName == loginForm.UserName)
                .Select(u => new
                {
                    u.UserName,
                    u.Password,
                    u.Id,
                    RoleName = u.UserRole.Role,                   
                })
                .FirstOrDefault();

            if (loginUser == null)
            {
                return new MessageDto<LoginResponse>
                {
                    Success = false,
                    Message = "المستخدم غير موجود"
                };
            }

            if (!VerifyPassword(loginForm.Password, loginUser.Password))
            {
                return new MessageDto<LoginResponse>
                {
                    Success = false,
                    Message = "كلمة المرور غير صحيحة"
                };
            }

            var token = GenerateJwtToken(loginUser.UserName, loginUser.RoleName);

            return new MessageDto<LoginResponse>
            {
                Success = true,
                Data = new LoginResponse
                {
                    Token = token,
                    RoleName = loginUser.RoleName,
                    UserId= loginUser.Id
                }
            };
        }

        private bool VerifyPassword(string providedPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }


        public MessageDto<LoginResponse> Register(RegisterForm registerForm)
        {
            var existingUser = _context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.UserName == registerForm.UserName);

            if (existingUser != null)
            {
                return new MessageDto<LoginResponse>
                {
                    Success = false,
                    Message = "اسم المستخدم مستخدم بالفعل"
                };
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerForm.Password);

            var newUser = new User
            { 
                FullName = registerForm.FullName,
                UserName = registerForm.UserName,
                Password = hashedPassword,
                RoleID = 2 
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            var role = _context.UserRole.Find(newUser.RoleID);
            if (role == null)
            {
                return new MessageDto<LoginResponse>
                {
                    Success = false,
                    Message = "فشل في استرداد دور المستخدم"
                };
            }
            var token = GenerateJwtToken(newUser.UserName, role.Role);

            return new MessageDto<LoginResponse>
            {
                Success = true,
                Message = "تم التسجيل بنجاح",
                Data = new LoginResponse
                {
                    Token = token,
                    RoleName = newUser.UserRole.Role,
                    UserId= newUser.Id
                }
            };
        }

        public MessageDto<string> Logout(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new MessageDto<string>
                {
                    Success = false,
                    Message = "التوكن غير صالح"
                };
            }

            var existingRevokedToken = _context.RevokedTokens.FirstOrDefault(rt => rt.Token == token);
            if (existingRevokedToken != null)
            {
                return new MessageDto<string>
                {
                    Success = false,
                    Message = "تم تسجيل الخروج بالفعل"
                };
            }

            var revokedToken = new RevokedToken
            {
                Token = token,
                RevokedAt = DateTime.Now
            };

            _context.RevokedTokens.Add(revokedToken);
            _context.SaveChanges();

            return new MessageDto<string>
            {
                Success = true,
                Message = "تم تسجيل الخروج بنجاح"
            };
        }

        private string GenerateJwtToken(string userName, string roleName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Role, roleName)
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
