using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services.Helper;
using Food_Management_System.Domain.Entities;
using Food_Management_System.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.InventoryService
{
    public class InventoryService:IInventoryService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IUserContext userContext;
        public InventoryService(IUnitOfWork _unitOfWork, IMapper _mapper,IUserContext _userContext)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            userContext = _userContext;
        }
        public async Task<IEnumerable<Inventory>?> GetAll()
        {
            return await unitOfWork.InventoryRepository.GetAll();
        }
        public async Task<Inventory?> GetById(int id)
        {
            return await unitOfWork.InventoryRepository.GetById(id);
        }
        public async Task<bool> Create(InventoryDto inventoryDto)
        {
            var inventory = mapper.Map<Inventory>(inventoryDto);
            inventory.CreatedDate = DateTime.UtcNow;
            inventory.ModifiedDate = DateTime.UtcNow;
            inventory.ModifiedBy = userContext.UserId ?? 0;
            inventory.CreatedBy = userContext.UserId ?? 0;
            await unitOfWork.InventoryRepository.Add(inventory);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<bool?> Update(int id, InventoryDto inventoryDto)
        {
            var inventory = await unitOfWork.InventoryRepository.GetById(id);
            if (inventory == null)
            {
                return null;
            }
            inventory.ModifiedDate = DateTime.UtcNow;
            inventory.ModifiedBy = userContext.UserId ?? 0;
            mapper.Map(inventoryDto, inventory);
            unitOfWork.InventoryRepository.Update(inventory);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<bool?> Delete(int id)
        {
            var inventory = await unitOfWork.InventoryRepository.GetById(id);
            if (inventory == null)
            {
                return null;
            }
            unitOfWork.InventoryRepository.Remove(inventory);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<IEnumerable<Inventory>?> GetLowStockItems()
        {
            return await unitOfWork.InventoryRepository.GetLowStockItem();
        }

    }
}
