﻿using AutoMapper;
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
        private const string MenuCacheKey = "Menu";
        public MenuService(IUnitOfWork _unitOfWork, IMapper _mapper,IUserContext _userContext,IMemoryCache _cache)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            userContext = _userContext;
            cache = _cache;
        }
        public async Task<IEnumerable<Menu>?> GetAll()
        {
            if (cache.TryGetValue(MenuCacheKey, out IEnumerable<Menu?> cachedMenus))
            {
                return cachedMenus;
            }
            var menus= await unitOfWork.MenuRepository.GetAll();
            cache.Set(MenuCacheKey, menus, cacheDuration);
            return menus;
        }
        public async Task<Menu?> GetById(int id)
        {
            string key = $"{MenuCacheKey}_{id}";
            if (cache.TryGetValue(key, out Menu? cachedMenu))
                return cachedMenu;
            var menu= await unitOfWork.MenuRepository.GetById(id);
            if (menu != null)
                cache.Set(key, menu, cacheDuration);
            return menu;
        }
        public async Task<bool> Create(MenuDto menuDto)
        {
            var menu = mapper.Map<Menu>(menuDto);
            menu.CreatedDate = DateTime.UtcNow;
            menu.ModifiedDate = DateTime.UtcNow;
            menu.CreatedBy = userContext.UserId ?? 0;
            menu.ModifiedBy = userContext.UserId ?? 0;
            await unitOfWork.MenuRepository.Add(menu);
            var changes = await unitOfWork.SaveChangesAsync()>0;
            if (changes)
            {
                cache.Remove(MenuCacheKey);
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cacheDuration
                };
                cache.Set($"{MenuCacheKey}_{menu.Id}", menu, cacheOptions);
            }
            return changes;
        }
        public async Task<bool?> Update(int id, MenuDto menuDto)
        {
            var menu = await unitOfWork.MenuRepository.GetById(id);
            if (menu == null)
            {
                return null;
            }
            menu.ModifiedDate = DateTime.UtcNow;
            menu.ModifiedBy = userContext.UserId ?? 0;
            mapper.Map(menuDto, menu);
            unitOfWork.MenuRepository.Update(menu);
            var changes = await unitOfWork.SaveChangesAsync()>0;
            if (changes)
            {
                cache.Remove(MenuCacheKey);
                cache.Remove($"{MenuCacheKey}_{id}");
                cache.Set($"{MenuCacheKey}_{id}", menu, cacheDuration);
            }
            return changes;
        }
        public async Task<bool?> Delete(int id)
        {
            var menu = await unitOfWork.MenuRepository.GetById(id);
            if (menu == null)
            {
                return null;
            }
            unitOfWork.MenuRepository.Remove(menu);
            var changes = await unitOfWork.SaveChangesAsync()>0;
            if (changes)
            {
                cache.Remove(MenuCacheKey);
                cache.Remove($"{MenuCacheKey}_{id}");
            }
            return changes;
        }
    }
}
