using Food_Management_System.Domain.Entities;
using Food_Management_System.Domain.Interfaces;
using Food_Management_System.Infrastructure.Data;
using Food_Management_System.Infrastructure.Extentions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Infrastructure.Repositories
{
    public class Repository<T>:IRepository<T> where T:class
    {
        private readonly AppDbContext context;
        private readonly DbSet<T> dbset;
        public Repository(AppDbContext _context)
        {
            context = _context;
            dbset = context.Set<T>();
        }
        public async Task<List<T>?> GetAll()
        {
            return await dbset.AsNoTracking().ToListAsync();
        }
        public async Task<Pagination<T>?> GetAll(int pageNumber, int pageSize)
        {
            return await dbset.AsNoTracking().ToPagedResultAsync(pageNumber, pageSize);
        }
        public async Task<T?> GetById(int id)
        {
            return await dbset.FindAsync(id);
        }
        public async Task Add(T entity)
        {
            await dbset.AddAsync(entity);
        }
        public void Update(T entity)
        {
            dbset.Update(entity);
        }
        public void Remove(T entity)
        {
            dbset.Remove(entity);
        }
    }
}
