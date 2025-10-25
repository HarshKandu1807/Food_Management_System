using Food_Management_System.Application.DTOS;
using Food_Management_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.UserService
{
    public interface IUserService
    {
        Task<IEnumerable<User>?> GetAll();
        Task<User?> GetById(int id);
        Task<string> Create(UserDto userDto);
        Task<string> Login(LoginDto dto);
        Task<bool?> Update(int id,UserDto userDto);
        Task<bool?> Delete(int id);
    }
}
