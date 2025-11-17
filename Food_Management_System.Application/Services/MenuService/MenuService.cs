using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services.Helper;
using Food_Management_System.Application.Services.MenuService;
using Food_Management_System.Domain.Entities;
using Food_Management_System.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache cache;
        private readonly TimeSpan cacheDuration = TimeSpan.FromMinutes(10);
        private readonly IUserContext userContext;
        public MenuService(IUnitOfWork _unitOfWork, IMapper _mapper,IUserContext _userContext,IMemoryCache _cache)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            userContext = _userContext;
            cache = _cache;
        }
        public async Task<Pagination<Menu>?> GetAll(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allItems = await unitOfWork.MenuRepository.GetAll();
                return new Pagination<Menu>
                {
                    Items = allItems,
                    PageNumber = 1,
                    PageSize = allItems.Count,
                    TotalCount = allItems.Count
                };
            }
            var MenuKey = $"Menu{pageNumber}_{pageSize}";
            if (cache.TryGetValue(MenuKey, out Pagination<Menu?> cachedMenus))
            {
                return cachedMenus;
            }
            var menus = await unitOfWork.MenuRepository.GetAll(pageNumber, pageSize);
            if (menus == null)
            {
                return null;
            }
            cache.Set(MenuKey, menus, cacheDuration);
            Console.WriteLine(DateTime.Now);
            return menus;
        }
        public async Task<Menu?> GetById(int id)
        {
            string key = $"Menu_{id}";
            if (cache.TryGetValue(key, out Menu? cachedMenu))
                return cachedMenu;
            var menu= await unitOfWork.MenuRepository.GetById(id);
            if (menu != null)
            {
                cache.Set(key, menu, cacheDuration);
            }
            else
            {
                return null;
            }
            return menu;
        }
        public async Task<bool> Create(MenuDto menuDto)
        {
            var menu = mapper.Map<Menu>(menuDto);
            menu.CreatedDate = DateTime.UtcNow;
            menu.ModifiedDate = DateTime.UtcNow;
            menu.CreatedBy = userContext.UserId ?? 0;
            menu.ModifiedBy = userContext.UserId ?? 0;
            var exist=await unitOfWork.MenuRepository.GetMenuByName(menuDto.MenuName);
            if (menuDto.Price <= 0)
            {
                return false;
            }
            if(exist != null)
            {
                return false;
            }
            await unitOfWork.MenuRepository.Add(menu);
            var changes = await unitOfWork.SaveChangesAsync()>0;
            return changes;
        }
        public async Task<bool?> Update(int id, MenuDto menuDto)
        {
            var menu = await unitOfWork.MenuRepository.GetById(id);
            if (menuDto.Price <= 0)
            {
                return false;
            }
            if (menu == null)
            {
                return false;
            }
            menu.ModifiedDate = DateTime.UtcNow;
            menu.ModifiedBy = userContext.UserId ?? 0;
            mapper.Map(menuDto, menu);
            unitOfWork.MenuRepository.Update(menu);
            var changes = await unitOfWork.SaveChangesAsync()>0;
            if (changes)
            {
                cache.Remove($"Menu_{id}");
            }
            return changes;
        }
        public async Task<bool?> Delete(int id)
        {
            var menu = await unitOfWork.MenuRepository.GetById(id);
            if (menu == null)
            {
                return false;
            }
            unitOfWork.MenuRepository.Remove(menu);
            var changes = await unitOfWork.SaveChangesAsync()>0;
            if (changes)
            {
                cache.Remove($"Menu_{id}");
            }
            return changes;
        }
    }
}
