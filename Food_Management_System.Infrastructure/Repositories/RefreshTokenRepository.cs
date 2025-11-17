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
    public class RefreshTokenRepository:Repository<RefreshToken>,IRefreshTokenRepository
    {
        private readonly AppDbContext context;

        public RefreshTokenRepository(AppDbContext _context):base(_context)
        {
            _context = _context;
        }
        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token && !r.IsActive && r.ExpiresAt>DateTime.UtcNow);
        }
    }
}
