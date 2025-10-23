using AutoMapper;
using Food_Management_System.Application.DTOS;
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
        public InventoryService(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
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
    }
}
