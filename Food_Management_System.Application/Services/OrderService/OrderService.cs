using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Domain.Entities;
using Food_Management_System.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.OrderService
{
    public class OrderService:IOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public OrderService(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public async Task<IEnumerable<Order>?> GetAll()
        {
            return await unitOfWork.OrderRepository.GetAll();
        }
        public async Task<Order?> GetById(int id)
        {
            return await unitOfWork.OrderRepository.GetById(id);
        }
        public async Task<bool> Create(OrderDto orderDto)
        {
            var order = mapper.Map<Order>(orderDto);
            order.CreatedDate = DateTime.UtcNow;
            order.ModifiedDate = DateTime.UtcNow;
            await unitOfWork.OrderRepository.Add(order);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<bool?> Update(int id, OrderDto orderDto)
        {
            var order = await unitOfWork.OrderRepository.GetById(id);
            if (order == null)
            {
                return null;
            }
            order.ModifiedDate = DateTime.UtcNow;
            mapper.Map(orderDto, order);
            unitOfWork.OrderRepository.Update(order);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<bool?> Delete(int id)
        {
            var order = await unitOfWork.OrderRepository.GetById(id);
            if (order == null)
            {
                return null;
            }
            unitOfWork.OrderRepository.Remove(order);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
    }
}
