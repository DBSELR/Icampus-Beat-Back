using ICampus_BusinessLogic.Interfaces;
using ICampus_Models.Requests;
using ICampus_Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ICampus_Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _svc;

        public LoginController(ILoginService svc)
        {
            _svc = svc;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "UserId and Password are required"
                });
            }

            var result = await _svc.AuthenticateAsync(request);

            if (result == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid credentials"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Login successful",
                Data = result
            });
        }
    }
}