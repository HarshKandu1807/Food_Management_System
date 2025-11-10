using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services.InventoryService;
using Food_Management_System.Application.Services.MenuService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
        {
            return Ok(await inventoryService.GetAll(pageNumber, pageSize));
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
            return Ok(await inventoryService.Create(inventoryDto));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] InventoryDto inventoryDto)
        {
            return Ok(await inventoryService.Update(id, inventoryDto));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await inventoryService.Delete(id);
            return Ok();
        }

        [HttpGet("GetLowStockItems")]
        public async Task<IActionResult> GetLowStockItem()
        {
            return Ok(await inventoryService.GetLowStockItems());
        }
    }
}
