using DDDProject.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using DDDProject.Domain.IRepositories.Auth;
using DDDProject.Domain.Entities;

namespace DDDProject.Infrastructure.Repositories.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public User? Login(string userName, string password)
        {
            var loginUser = _context.Users
                .AsNoTracking()
                .Include(u => u.UserRole)
                .FirstOrDefault(u => u.UserName == userName);

            if (loginUser == null || !VerifyPassword(password, loginUser.Password))
            {
                return null;
            }

            return loginUser; 
        }

        public User? GetUserByUserName(string userName)
        {
            return _context.Users
                .AsNoTracking()
                .Include(u => u.UserRole)
                .FirstOrDefault(u => u.UserName == userName);
        }

        public User Register(User newUser)
        {
            _context.Users.Add(newUser);
            _context.SaveChanges();

            return _context.Users
                .Include(u => u.UserRole)
                .FirstOrDefault(u => u.Id == newUser.Id)!;
        }

        private bool VerifyPassword(string providedPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }


        public bool Logout(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var existingRevokedToken = _context.RevokedTokens.FirstOrDefault(rt => rt.Token == token);
            if (existingRevokedToken != null)
            {
                return false; 
            }
            var revokedToken = new RevokedToken
            {
                Token = token,
                RevokedAt = DateTime.UtcNow
            };
            _context.RevokedTokens.Add(revokedToken);
            _context.SaveChanges();

            return true;
        }
    }
}
