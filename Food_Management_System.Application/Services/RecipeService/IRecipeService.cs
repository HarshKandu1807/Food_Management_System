using Food_Management_System.Application.DTOS;
using Food_Management_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.RecipeService
{
    public interface IRecipeService
    {
        Task<Pagination<Recipe>?> GetAll(int pageNumber, int pageSize);
        Task<Recipe?> GetById(int id);
        Task<bool> Create(RecipeDto recipeDto);
        Task<bool?> Update(int id, RecipeDto recipeDto);
        Task<bool?> Delete(int id);
    }
}
