using Food_Management_System.Application.DTOS;
using Food_Management_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        Task<(string, string)> Login(LoginDto dto);
        string GenerateAccessToken(User user);
        Task ExpireToken(string token);
        Task<(string, string)> RevokeAsync(string token);
    }
}
