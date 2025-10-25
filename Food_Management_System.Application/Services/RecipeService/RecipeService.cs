using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services.Helper;
using Food_Management_System.Domain.Entities;
using Food_Management_System.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.RecipeService
{
    public class RecipeService:IRecipeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IUserContext userContext;
        public RecipeService(IUnitOfWork _unitOfWork, IMapper _mapper,IUserContext _userContext)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            userContext = _userContext;
        }
        public async Task<IEnumerable<Recipe>?> GetAll()
        {
            return await unitOfWork.RecipeRepository.GetAll();
        }
        public async Task<Recipe?> GetById(int id)
        {
            return await unitOfWork.RecipeRepository.GetById(id);
        }
        public async Task<bool> Create(RecipeDto recipeDto)
        {
            var recipe = mapper.Map<Recipe>(recipeDto);
            recipe.CreatedDate = DateTime.UtcNow;
            recipe.ModifiedDate = DateTime.UtcNow;
            recipe.CreatedBy = userContext.UserId ?? 0;
            recipe.ModifiedBy = userContext.UserId ?? 0;
            await unitOfWork.RecipeRepository.Add(recipe);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<bool?> Update(int id, RecipeDto recipeDto)
        {
            var recipe = await unitOfWork.RecipeRepository.GetById(id);
            if (recipe == null)
            {
                return null;
            }
            recipe.ModifiedDate = DateTime.UtcNow;
            recipe.ModifiedBy = userContext.UserId ?? 0;
            mapper.Map(recipeDto, recipe);
            unitOfWork.RecipeRepository.Update(recipe);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<bool?> Delete(int id)
        {
            var recipe = await unitOfWork.RecipeRepository.GetById(id);
            if (recipe == null)
            {
                return null;
            }
            unitOfWork.RecipeRepository.Remove(recipe);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
    }
}
