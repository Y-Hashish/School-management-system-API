using School_management_system.Models;

namespace School_management_system.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> GenerateToken(LoginModel model);
    }
}
