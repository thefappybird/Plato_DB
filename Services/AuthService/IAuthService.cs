using Microsoft.AspNetCore.Mvc;
using Plato_DB.Dtos.User;
using Plato_DB.Interfaces;
using Plato_DB.Models;

namespace Plato_DB.Services.AuthService
{
    public interface IAuthService
    {
        Task<IRegisterResponse> RegisterAsync(RegisterUserDto request);
        Task<string?> LoginAsync(LoginUserDto request);
        Task<UserDto?> FetchUser(Guid userId);
    }
}
