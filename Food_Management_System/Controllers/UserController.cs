using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services;
using Food_Management_System.Application.Services.UserService;
using Food_Management_System.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService service;
        private readonly IMapper mapper;
        public UserController(IUserService _service, IMapper _mapper) { 
            service = _service;
            mapper = _mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() { 
            return Ok(await service.GetAll()); 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await service.GetById(id);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto userDto)
        {
            await service.Create(userDto);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await service.Login(dto);
            return Ok(new { token });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDto user)
        {
            await service.Update(id,user);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await service.Delete(id);
            return NoContent();
        }
    }
}
