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
    public class OrderRepository:Repository<Order>,IOrderRepository
    {
        private readonly AppDbContext dbContext;
        public OrderRepository(AppDbContext _dbContext) : base(_dbContext)
        {
            dbContext = _dbContext;
        }

        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        {
            return await dbContext.Orders.Include(o => o.OrderDetails).ThenInclude(m => m.Menu).FirstOrDefaultAsync(o => o.Id == orderId);
        }
        public async Task<IEnumerable<Order>?> GetDailyOrderReport(DateTime date)
        {
            return await dbContext.Orders.Include(o=>o.OrderDetails).ThenInclude(m=>m.Menu).Where(d => d.OrderDate.Date == date.Date).ToListAsync();
        }
    }
}
