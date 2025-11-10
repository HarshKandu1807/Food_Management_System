using Food_Management_System.Application.DTOS;
using Food_Management_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.MenuService
{
    public interface IMenuService
    {
        Task<Pagination<Menu>?> GetAll(int pageNumber, int pageSize);
        Task<Menu?> GetById(int id);
        Task<bool> Create(MenuDto menuDto);
        Task<bool?> Update(int id, MenuDto menuDto);
        Task<bool?> Delete(int id);
    }
}
