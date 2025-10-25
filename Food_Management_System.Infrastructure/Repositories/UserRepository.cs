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
    public class UserRepository: Repository<User>,IUserRepository
    {
        private readonly AppDbContext dbContext;
        public UserRepository(AppDbContext _dbContext):base(_dbContext)
        {
            dbContext = _dbContext;
        }
        public async Task<User?> FindByContact(int contactNo)
        {
            return await dbContext.Users.FirstOrDefaultAsync(x=>x.ContactNo==contactNo);
        }
    }
}
