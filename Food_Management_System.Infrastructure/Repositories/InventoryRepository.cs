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
    public class InventoryRepository:Repository<Inventory>,IInventoryRepository
    {
        private readonly AppDbContext dbContext;
        public InventoryRepository(AppDbContext _dbContext) : base(_dbContext)
        {
            dbContext = _dbContext;
        }
        public async Task<IEnumerable<Inventory>?> GetByIds(List<int> ids)
        {
            return await dbContext.Inventory.Where(i => ids.Contains(i.Id)).ToListAsync();
        }
        public async Task<IEnumerable<Inventory>?> GetLowStockItem()
        {
            return await dbContext.Inventory.Where(i => i.QuantityAvailable < i.ReorderLevel).ToListAsync();
        }
    }
}
