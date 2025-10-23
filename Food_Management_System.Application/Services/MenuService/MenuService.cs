using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services.MenuService;
using Food_Management_System.Domain.Entities;
using Food_Management_System.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.MenuService
{
    public class MenuService:IMenuService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public MenuService(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public async Task<IEnumerable<Menu>?> GetAll()
        {
            return await unitOfWork.MenuRepository.GetAll();
        }
        public async Task<Menu?> GetById(int id)
        {
            return await unitOfWork.MenuRepository.GetById(id);
        }
        public async Task<bool> Create(MenuDto menuDto)
        {
            var menu = mapper.Map<Menu>(menuDto);
            menu.CreatedDate = DateTime.UtcNow;
            menu.ModifiedDate = DateTime.UtcNow;
            await unitOfWork.MenuRepository.Add(menu);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<bool?> Update(int id, MenuDto menuDto)
        {
            var menu = await unitOfWork.MenuRepository.GetById(id);
            if (menu == null)
            {
                return null;
            }
            menu.ModifiedDate = DateTime.UtcNow;
            mapper.Map(menuDto, menu);
            unitOfWork.MenuRepository.Update(menu);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<bool?> Delete(int id)
        {
            var menu = await unitOfWork.MenuRepository.GetById(id);
            if (menu == null)
            {
                return null;
            }
            unitOfWork.MenuRepository.Remove(menu);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
    }
}
