
using DapperApiDemo.Helpers;
using DapperApiDemo.Models;
using DapperApiDemo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DapperApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;

        public AuthController(IUserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            var existingUser = await _userRepo.GetByUsername(user.Username);
            if (existingUser != null)
                return BadRequest("Username already exists.");

            await _userRepo.Register(user);
            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(User user)
        {
            var dbUser = await _userRepo.GetByUsername(user.Username);
            if (dbUser == null || dbUser.Password != user.Password)
                return Unauthorized("Invalid credentials.");

            var jwtKey = _config["Jwt:Key"];
            var token = JwtHelper.GenerateJwtToken(user.Username, jwtKey!);

            return Ok(new { Token = token });
        }
    }
}
