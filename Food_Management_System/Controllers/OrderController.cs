using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services.MenuService;
using Food_Management_System.Application.Services.OrderService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderService;
        private readonly IMapper mapper;

        public OrderController(IMapper _mapper, IOrderService _orderService)
        {
            orderService = _orderService;
            mapper = _mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await orderService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var menu = await orderService.GetById(id);
            return menu is null ? NotFound() : Ok(menu);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderDto orderDto)
        {
            var created = await orderService.Create(orderDto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderDto orderDto)
        {
            await orderService.Update(id, orderDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await orderService.Delete(id);
            return Ok();
        }
    }
}
