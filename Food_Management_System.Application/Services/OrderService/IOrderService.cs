using Food_Management_System.Application.DTOS;
using Food_Management_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.OrderService
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>?> GetAll();
        Task<Order?> GetById(int id);
        Task<bool> Create(OrderDto orderDto);
        Task<bool?> Update(int id, OrderDto orderDto);
        Task<bool?> Delete(int id);
    }
}
