
using AutoMapper;
using DapperApi.DTO;
using DapperApi.IRepositories;
using DapperApiDemo.Helpers;
using DapperApiDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace DapperApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IUserRepository userRepo, IConfiguration config, IMapper mapper)
        {
            _userRepo = userRepo;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            var user = _mapper.Map<User>(dto);

            var existingUser = await _userRepo.GetByUsername(user.Username);
            if (existingUser != null)
                return BadRequest("Username already exists.");

            // Hash the password before saving
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.IsActive ??= true;


            var userId = await _userRepo.Register(user);

            if (userId == -1)
                return BadRequest("Username already exists.");


            return Ok(new { Message = "User registered successfully", UserId = userId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var dbUser = await _userRepo.GetByUsername(login.Username);
            if (dbUser == null || !BCrypt.Net.BCrypt.Verify(login.Password, dbUser.Password))
                return Unauthorized("Invalid credentials.");


            var jwtKey = _config["Jwt:Key"];
            var token = JwtHelper.GenerateJwtToken(dbUser.Username, jwtKey!);

            return Ok(new { Token = token,dbUser });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetDto)
        {
            var dbUser = await _userRepo.GetByUsername(resetDto.Username);
            if (dbUser == null)
                return NotFound("User not found.");
            // Hash the new password before updating
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(resetDto.NewPassword);
            var updated = await _userRepo.UpdatePassword(dbUser.UserId, hashedPassword);
            if (!updated)
                return BadRequest("Failed to update password.");
            return Ok("Password updated successfully.");
        }
    }
}
