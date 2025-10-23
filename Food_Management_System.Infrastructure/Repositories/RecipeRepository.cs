using Food_Management_System.Domain.Entities;
using Food_Management_System.Domain.Interfaces;
using Food_Management_System.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Infrastructure.Repositories
{
    public class RecipeRepository:Repository<Recipe>,IRecipeRepository
    {
        private readonly AppDbContext dbContext;
        public RecipeRepository(AppDbContext _dbContext):base(_dbContext)
        {
            dbContext = _dbContext;
        }
    }
}
