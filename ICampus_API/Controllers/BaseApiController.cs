using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ICampus_Api.Controllers
{
    [Authorize]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        protected string UserGroup => User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        protected string Regulation => User.FindFirstValue("Regulation") ?? string.Empty;
        protected string Course => User.FindFirstValue("Course") ?? string.Empty;
        protected string Theme => User.FindFirstValue("Theme") ?? string.Empty;
    }
}