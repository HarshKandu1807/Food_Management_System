using Food_Management_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Domain.Interfaces
{
    public interface IInventoryRepository:IRepository<Inventory>
    {
        Task<IEnumerable<Inventory>?> GetByIds(List<int> ids);
        Task<IEnumerable<Inventory>?> GetLowStockItem();
    }
}
