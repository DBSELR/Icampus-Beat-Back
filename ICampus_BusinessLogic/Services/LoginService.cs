using ICampus_BusinessLogic.Interfaces;
using ICampus_DataAccessLayer.Helpers;
using ICampus_DataAccessLayer.Interfaces;
using ICampus_Models.DTOs;
using ICampus_Models.Requests;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ICampus_BusinessLogic.Services
{
    public class LoginService : ILoginService
    {
        private readonly IGenericRepository<LoginDto> _repo;
        private readonly IConfiguration _config;

        public LoginService(IGenericRepository<LoginDto> repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        public async Task<LoginResponseDto?> AuthenticateAsync(LoginRequest request)
        {
            var pUserId = new SqlParameter("@USERID", SqlDbType.VarChar) { Value = request.UserId };
            var pPwd = new SqlParameter("@PWD", SqlDbType.VarChar) { Value = request.Password };
            var pMac = new SqlParameter("@MACADD", SqlDbType.VarChar) { Value = request.MacAddress ?? "Y" };

            var sql = StoredProcSql.Exec(StoredProcedures.SP_LOGIN_CHECK, "@USERID", "@PWD", "@MACADD");
            var result = await _repo.QueryFromStoredProcAsync(sql, pUserId, pPwd, pMac);

            var user = result.FirstOrDefault();
            if (user == null) return null;

            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Token = token,
                User = user
            };
        }

        private string GenerateJwtToken(LoginDto user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Role, user.UserGroup ?? string.Empty),
                new Claim("Theme", user.Theme ?? string.Empty),
                new Claim("Course", user.Course ?? string.Empty),
                new Claim("Regulation", user.Regulation ?? string.Empty)
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
