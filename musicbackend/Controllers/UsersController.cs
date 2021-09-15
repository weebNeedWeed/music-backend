using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using musicbackend.Common;
using musicbackend.Data;
using musicbackend.DTOs;
using musicbackend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace musicbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MusicContext _context;
        public UsersController(MusicContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Register([FromBody]UserRegisterDto userRegisterDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            User newUser = new User
            {
                Username = userRegisterDto.Username,
                Email = userRegisterDto.Email,
                Password = new AuthHelper().HashPassword(userRegisterDto.Password),
            };

            if(await IsUserExisted(userRegisterDto.Email))
            {
                return Conflict();
            }

            try
            {
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict();
            }
            catch (Exception)
            {
                throw;
            }

            return newUser;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<User>> GetUser(){
            int userId = Int32.Parse(User.Claims.First(x => x.Type == "Id").Value); 

            User user = await _context.Users.FindAsync(userId);

            if(user == null){
                return NotFound();
            }

            return user;
        }

        private async Task<bool> IsUserExisted(string email)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            return (user != null);
        }
    }
}
