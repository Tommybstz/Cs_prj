using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RecipeAPI.Data;
using RecipeAPI.DTOs;
using RecipeAPI.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RecipeAPI.Services
{
    public class AuthService
    {
        private AppDbContext _db;
        private ILogger<AuthService> _logger;
        private IConfiguration _config;

        public AuthService(AppDbContext db, ILogger<AuthService> logger, IConfiguration config)
        {
            _db = db;
            _logger = logger;
            _config = config;
        }
        public bool Register(RegisterRequest req)
        {

            if (_db.Users.Any(u => u.Username == req.Username))
            {
                _logger.LogWarning("User with username {Username} already exists.", req.Username);
                return false;
            }

            var Hasher = new PasswordHasher<User>();
            var user = new User();

            user.Username = req.Username;
            user.Role = Role.User;
            user.PasswordHash = Hasher.HashPassword(user, req.Password);

            _logger.LogInformation("Attempting to create new user:{Username}.", req.Username);
            _db.Users.Add(user);
            _logger.LogInformation("User with username {Username} added to memory.", req.Username);

            try
            {
                _db.SaveChanges();
                _logger.LogInformation("User with username {Username} added to database.", req.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving user:{Username} to database.", req.Username);
                throw;
            }

            return true;
        }
        public string? Login(LoginRequest req)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == req.Username);
            if (user == null) 
            {
                _logger.LogWarning("Login attempt with non-existent user");
                return null;
            }
            _logger.LogInformation("Login attempt by User: {Username}",user.Username);

            var hasher = new PasswordHasher<User>();

            var isValidPassword = hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);

            if (isValidPassword == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("User:{Username} tried to log in with an invalid password",user.Username);
                return null;
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.Role,user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims:claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
                );

            _logger.LogInformation("User: {Username} logged in successefully",user.Username);
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
