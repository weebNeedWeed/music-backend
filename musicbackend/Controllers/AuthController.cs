using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using musicbackend.Common;
using musicbackend.Data;
using musicbackend.DTOs;
using musicbackend.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace musicbackend.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MusicContext _context;

        public AuthController(IConfiguration configuration, MusicContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Auth([FromBody]UserAuthDto userAuthDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            string hashPassword = new AuthHelper().HashPassword(userAuthDto.Password);

            User user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userAuthDto.Email && x.Password == hashPassword);

            if (user == null)
            {
                return BadRequest("Invalid credentials");
            }

            var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Id", user.UserId.ToString()),
                    new Claim("UserName", user.Username),
                    new Claim("Email", user.Email),
                   };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Issuer"], claims, expires: DateTime.UtcNow.AddHours(3), signingCredentials: signIn);

            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
