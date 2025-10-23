using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services.MenuService;
using Food_Management_System.Application.Services.RecipeService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService recipeService;
        private readonly IMapper mapper;

        public RecipeController(IMapper _mapper, IRecipeService _recipeService)
        {
            recipeService = _recipeService;
            mapper = _mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await recipeService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var menu = await recipeService.GetById(id);
            return menu is null ? NotFound() : Ok(menu);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RecipeDto recipeDto)
        {
            var created = await recipeService.Create(recipeDto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RecipeDto recipeDto)
        {
            await recipeService.Update(id, recipeDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await recipeService.Delete(id);
            return Ok();
        }
    }
}
