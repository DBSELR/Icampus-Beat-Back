using ICampus_Models.DTOs;
using ICampus_Models.Requests;

namespace ICampus_BusinessLogic.Interfaces
{
    public interface ILoginService
    {
        Task<LoginResponseDto?> AuthenticateAsync(LoginRequest request);
    }
}
