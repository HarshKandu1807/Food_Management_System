using Food_Management_System.Domain.Entities;
using Food_Management_System.Domain.Interfaces;
using Food_Management_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Infrastructure.Repositories
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly AppDbContext context;
        public IUserRepository UserRepository { get; }
        public IMenuRepository MenuRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IInventoryRepository InventoryRepository { get; }
        public IRecipeRepository RecipeRepository { get; }
        public UnitOfWork(AppDbContext _context, IUserRepository _UserRepository,IMenuRepository _MenuRepository, 
            IOrderRepository _OrderRepository, IInventoryRepository _InventoryRepository, IRecipeRepository _RecipeRepository)
        {
            context = _context;
            UserRepository = _UserRepository;
            MenuRepository = _MenuRepository;
            OrderRepository = _OrderRepository;
            InventoryRepository = _InventoryRepository;
            RecipeRepository = _RecipeRepository;
        }
        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
