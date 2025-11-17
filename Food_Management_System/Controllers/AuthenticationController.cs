using AutoMapper;
using Azure.Core;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services.AuthenticationService;
using Food_Management_System.Application.Services.UserService;
using Food_Management_System.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService service;
        public AuthenticationController(IAuthenticationService _service)
        {
            service = _service;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await service.Login(dto);
            return Ok(new { 
                AccessToken=token.Item1,
                RefreshToken=token.Item2
            });
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
        {
            await service.ExpireToken(dto.RefreshToken);
            return Ok(new { message = "Logged out successfully" });
        }
        [HttpPost("TokenGeneration")]
        public async Task<IActionResult> TokenGeneration([FromBody] LogoutDto dto)
        {
            var token=await service.RevokeAsync(dto.RefreshToken);
            return Ok(new {
                AccessToken=token.Item1,
                RefreshToken=token.Item2
            });
        }

    }
}
