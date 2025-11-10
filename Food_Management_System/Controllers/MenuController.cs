using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services;
using Food_Management_System.Application.Services.MenuService;
using Food_Management_System.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService menuService;
        private readonly IMapper mapper;

        public MenuController(IMapper _mapper,IMenuService _menuService)
        {
            menuService = _menuService;
            mapper = _mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
        {
            return Ok(await menuService.GetAll(pageNumber, pageSize));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var menu = await menuService.GetById(id);
            return menu is null ? NotFound() : Ok(menu);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MenuDto menuDto)
        {
            var created = await menuService.Create(menuDto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MenuDto menuDto)
        {
            await menuService.Update(id,menuDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await menuService.Delete(id);
            return Ok();
        }
    }
}
