
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Plato_DB.Data;
using Plato_DB.Dtos.User;
using Plato_DB.Interfaces;
using Plato_DB.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Plato_DB.Services.AuthService
{
    public class AuthService(AppDbContext context, IConfiguration configuration, IMapper mapper) : IAuthService
    {
        public async Task<IRegisterResponse> RegisterAsync(RegisterUserDto request)
        {
            if (await context.Users.AnyAsync(u => u.Username == request.Username))
            {       
                return new RegisterResponse{
                    Passed = false,
                    Message= "Username Already Exists."
                };
            }
            if(await context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return new RegisterResponse
                {
                    Passed = false,
                    Message = "Email Already Exists."
                };
            }
            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
            .HashPassword(user, request.Password);
            user.Username = request.Username;
            user.Email = request.Email;
            user.PasswordHash = hashedPassword;

            context.Users.Add(user);
            await context.SaveChangesAsync();
            return new RegisterResponse
            {
                Passed = true,
                Message = "Successfully registered new user"
            };
        }
        public async Task<string?> LoginAsync(LoginUserDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.LoginIdentifier || u.Email == request.LoginIdentifier);
            if (user == null)
            {
                return null;
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return CreateToken(user);
        }
        public async Task<UserDto?> FetchUser(Guid userId)
        {
            var user = await context.Users
                .Where(u => u.UserId == userId)
                .Include(u => u.Recipes)
                .Include(u => u.Favorites)
                    .ThenInclude(f => f.Recipe)
                        .ThenInclude(r => r.User)
                .Include(u => u.Ratings)
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            var userDto = mapper.Map<UserDto>(user, opt =>
            {
                opt.Items["CurrentUserId"] = userId;
            });

            return userDto;
        }


        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
