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
    public class MenuController : ControllerBase
    {
        private readonly IBaseService<Menu> service;
        private readonly IMapper mapper;

        public MenuController(IBaseService<Menu> _service,IMapper _mapper)
        {
            service = _service;
            mapper = _mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
             return Ok(await service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var menu = await service.GetById(id);
            return menu is null ? NotFound() : Ok(menu);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MenuDto menuDto)
        {
            var menu = mapper.Map<Menu>(menuDto);
            var created = await service.Create(menu);
            var result = mapper.Map<MenuDto>(created);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Menu menu)
        {
            if (id != menu.Id) return BadRequest();
            await service.Update(menu);
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
