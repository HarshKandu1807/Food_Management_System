using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services;
using Food_Management_System.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IBaseService<User> service;
        private readonly IMapper mapper;
        public UserController(IBaseService<User> _service, IMapper _mapper) { 
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
            var user = mapper.Map<User>(userDto);
            var created = await service.Create(user);
            var result = mapper.Map<UserDto>(created);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            if (id != user.Id) return BadRequest();
            await service.Update(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await service.Delete(id);
            return NoContent();
        }
    }
}
