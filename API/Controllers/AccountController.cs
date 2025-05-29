using API.Database;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController(UserManager<AppUser> userManager, IUserRepository userRepository, 
        ITokenService tokenService, IMapper mapper) : BaseApiController
    {
        [HttpPost("register")]  //account register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await UserExits(registerDto.Username)) return BadRequest("Username is taken.");

            var user = mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.Username.ToLower();

            var result = await userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return new UserDto
            {
                Username = user.UserName,
                Gender = user.Gender,
                Token = await tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };  
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // var user = await dbContext.Users.Where(x => x.UserName == loginDto.Username.ToLower()).FirstOrDefaultAsync();
            var user = await userManager.Users
                .Include(p => p.Photos)
                    .FirstOrDefaultAsync(x =>
                        x.NormalizedUserName == loginDto.Username.ToUpper());

            if (user is null || user.UserName is null) 
                return Unauthorized("Invalid username.");

            var result = await userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result) return Unauthorized();
            
            return new UserDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Gender = user.Gender,
                Token = await tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
        }

        private async Task<bool> UserExits(string username)
        {
            return await userManager.Users.AnyAsync<AppUser>(x => x.NormalizedUserName == username.ToUpper());
        }
    }

}  
