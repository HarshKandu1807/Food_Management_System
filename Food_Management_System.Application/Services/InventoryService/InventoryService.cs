using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services.EmailService;
using Food_Management_System.Application.Services.Helper;
using Food_Management_System.Domain.Entities;
using Food_Management_System.Domain.Interfaces;
using Food_Management_System.Infrastructure.Repositories;
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
        private readonly IEmailService emailService;
        public InventoryService(IUnitOfWork _unitOfWork, IMapper _mapper,IUserContext _userContext,IEmailService _emailService)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            userContext = _userContext;
            emailService = _emailService;
        }
        public async Task<IEnumerable<Inventory>?> GetAll()
        {
            var items= await unitOfWork.InventoryRepository.GetAll();
            if (items == null)
            {
                return null;
            }
            return items;
        }
        public async Task<Inventory?> GetById(int id)
        {
            var item= await unitOfWork.InventoryRepository.GetById(id);
            if (item == null)
            {
                return null;
            }
            return item;
        }
        public async Task<bool?> Create(InventoryDto inventoryDto)
        {
            if (inventoryDto.QuantityAvailable < 0)
            {
                return false;
            }
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
            if (inventoryDto.QuantityAvailable < 0)
            {
                return false;
            }
            if (inventory == null)
            {
                return false;
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
                return false;
            }
            unitOfWork.InventoryRepository.Remove(inventory);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<IEnumerable<Inventory>?> GetLowStockItems()
        {
            var lowStockItems = await unitOfWork.InventoryRepository.GetLowStockItem();
            if (!lowStockItems.Any())
                return null;

            var sb = new StringBuilder();
            sb.Append("<h2>Low Stock Alert </h2>");
            sb.Append("<p>The following inventory items are below their reorder levels:</p>");
            sb.Append("<table border='1' cellspacing='0' cellpadding='5'>");
            sb.Append("<tr><th>Item Name</th><th>Unit</th><th>Available</th><th>Reorder Level</th></tr>");

            foreach (var item in lowStockItems)
            {
                sb.Append($"<tr>");
                sb.Append($"<td>{item.ItemName}</td>");
                sb.Append($"<td>{item.Unit}</td>");
                sb.Append($"<td>{Math.Round(item.QuantityAvailable, 1)}</td>");
                sb.Append($"<td>{item.ReorderLevel}</td>");
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            var email = "harsh.kandu@nimapinfotech.com";

            await emailService.SendEmail(
                email,
                "Low Stock Alert - Food Management System",
                sb.ToString()
            );
            return lowStockItems;
        }

    }
}
