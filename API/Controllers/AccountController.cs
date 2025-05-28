using API.Database;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController(DataContext dbContext, IUserRepository userRepository, 
        ITokenService tokenService, IMapper mapper) : BaseApiController
    {
        [HttpPost("register")]  //account register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await UserExits(registerDto.Username)) return BadRequest("Username is taken.");

            using var hmac = new HMACSHA512();

            var user = mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            dbContext.Users.Add(user);
            int count = await dbContext.SaveChangesAsync();

            if (count == 0)
                return StatusCode(500, "Error while adding to database.");

            return new UserDto
            {
                Username = user.UserName,
                Gender = user.Gender,
                Token = tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };  
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // var user = await dbContext.Users.Where(x => x.UserName == loginDto.Username.ToLower()).FirstOrDefaultAsync();
            var user = await userRepository.GetUserByUsernameAsync(loginDto.Username);

            if (user == null)
                return Unauthorized("Invalid username.");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i = 0; i < computedhash.Length; i++)
            {
                if (computedhash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid password.");
            }
            
            return new UserDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Gender = user.Gender,
                Token = tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
        }

        private async Task<bool> UserExits(string username)
        {
            return await dbContext.Users.AnyAsync<AppUser>(x => x.UserName.ToLower() == username.ToLower());
        }
    }

}  
