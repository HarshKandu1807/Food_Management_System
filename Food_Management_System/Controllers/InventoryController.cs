using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services.InventoryService;
using Food_Management_System.Application.Services.MenuService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService inventoryService;
        private readonly IMapper mapper;

        public InventoryController(IMapper _mapper, IInventoryService _inventoryService)
        {
            inventoryService = _inventoryService;
            mapper = _mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await inventoryService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var menu = await inventoryService.GetById(id);
            return menu is null ? NotFound() : Ok(menu);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InventoryDto inventoryDto)
        {
            var created = await inventoryService.Create(inventoryDto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] InventoryDto inventoryDto)
        {
            await inventoryService.Update(id, inventoryDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await inventoryService.Delete(id);
            return Ok();
        }
    }
}
