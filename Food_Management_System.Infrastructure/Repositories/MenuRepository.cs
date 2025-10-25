using Food_Management_System.Domain.Entities;
using Food_Management_System.Domain.Interfaces;
using Food_Management_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Infrastructure.Repositories
{
    public class MenuRepository:Repository<Menu>,IMenuRepository
    {
        private readonly AppDbContext dbContext;
        public MenuRepository(AppDbContext _dbcontext):base(_dbcontext)
        {
            dbContext = _dbcontext;
        }
        public async Task<Menu?> GetMenuWithRecipeAsync(int menuId)
        {
            return await dbContext.Menu.Include(x => x.Recipes).ThenInclude(x => x.Inventory).FirstOrDefaultAsync(x => x.Id == menuId);
        }
    }
}
