using Food_Management_System.Application.DTOS;
using Food_Management_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.InventoryService
{
    public interface IInventoryService
    {
        Task<IEnumerable<Inventory>?> GetAll();
        Task<Inventory?> GetById(int id);
        Task<bool> Create(InventoryDto inventoryDto);
        Task<bool?> Update(int id, InventoryDto inventoryDto);
        Task<bool?> Delete(int id);
        Task<IEnumerable<Inventory>?> GetLowStockItems();
    }
}
