using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services.EmailService;
using Food_Management_System.Application.Services.Helper;
using Food_Management_System.Domain.Entities;
using Food_Management_System.Domain.Interfaces;
using Food_Management_System.Infrastructure.Repositories;
using MimeKit;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.OrderService
{
    public class OrderService:IOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IUserContext userContext;
        private readonly IEmailService emailService;
        public OrderService(IUnitOfWork _unitOfWork, IMapper _mapper, IUserContext _userContext, IEmailService _emailService)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            userContext = _userContext;
            emailService = _emailService;
        }
        public async Task<IEnumerable<Order>?> GetAll()
        {
            return await unitOfWork.OrderRepository.GetAll();
        }
        public async Task<Order?> GetById(int id)
        {
            return await unitOfWork.OrderRepository.GetById(id);
        }
        public async Task<bool?> Create(OrderDto orderDto)
        {
            if (orderDto.OrderDetails == null || !orderDto.OrderDetails.Any())
                return false;

            double totalAmount = 0;
            var requiredItems = new Dictionary<int, double>();

            var order = new Order
            {
                UserId=orderDto.UserId,
                CustomerName = orderDto.CustomerName,
                OrderDate = DateTime.UtcNow,
                TotalAmount = 0,
                CreatedDate=DateTime.UtcNow,
                ModifiedDate=DateTime.UtcNow,
                CreatedBy = userContext.UserId ?? 0,
                ModifiedBy = userContext.UserId ?? 0,
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var item in orderDto.OrderDetails)
            {
                var menu = await unitOfWork.MenuRepository.GetMenuWithRecipeAsync(item.MenuId);
                if (menu == null)
                    return false;

                totalAmount += menu.Price * item.QuantityOrdered;

                order.OrderDetails.Add(new OrderDetail
                {
                    MenuId = item.MenuId,
                    QuantityOrdered = item.QuantityOrdered,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    CreatedBy = userContext.UserId ?? 0,
                    ModifiedBy = userContext.UserId ?? 0
                });

                foreach (var recipe in menu.Recipes)
                {
                    double needed = recipe.QuantityRequired * item.QuantityOrdered;
                    if (requiredItems.ContainsKey(recipe.ItemId))
                        requiredItems[recipe.ItemId] += needed;
                    else
                        requiredItems.Add(recipe.ItemId, needed);
                }
            }

            var ingredientIds = requiredItems.Keys.ToList();
            var inventoryItems = await unitOfWork.InventoryRepository.GetByIds(ingredientIds);

            foreach (var i in inventoryItems)
            {
                double needed = requiredItems[i.Id];
                if (i.QuantityAvailable < needed)
                    return false;

                i.QuantityAvailable -= needed;
                i.ModifiedDate = DateTime.UtcNow;
                i.ModifiedBy = userContext.UserId ?? 0;
                unitOfWork.InventoryRepository.Update(i);
            }

            order.TotalAmount = totalAmount;

            await unitOfWork.OrderRepository.Add(order);

            var changes=await unitOfWork.SaveChangesAsync()>0;

            return changes;
        }

        public async Task<bool?> Update(int orderId, OrderDto updatedOrderDto)
        {
            var existingOrder = await unitOfWork.OrderRepository.GetOrderWithDetailsAsync(orderId);
            if (existingOrder == null)
                return false;

            foreach (var oldDetail in existingOrder.OrderDetails)
            {
                var menu = await unitOfWork.MenuRepository.GetMenuWithRecipeAsync(oldDetail.MenuId);
                foreach (var recipe in menu.Recipes)
                {
                    var inventory = await unitOfWork.InventoryRepository.GetById(recipe.ItemId);
                    inventory.QuantityAvailable += recipe.QuantityRequired * oldDetail.QuantityOrdered;
                    inventory.ModifiedDate = DateTime.UtcNow;
                    unitOfWork.InventoryRepository.Update(inventory);
                }
            }

            existingOrder.OrderDetails.Clear();

            double newTotal = 0;
            var requiredIngredients = new Dictionary<int, double>();

            foreach (var item in updatedOrderDto.OrderDetails)
            {
                var menu = await unitOfWork.MenuRepository.GetMenuWithRecipeAsync(item.MenuId);
                if (menu == null)
                    return false;

                newTotal += menu.Price * item.QuantityOrdered;

                existingOrder.OrderDetails.Add(new OrderDetail
                {
                    MenuId = item.MenuId,
                    QuantityOrdered = item.QuantityOrdered,
                    CreatedDate = existingOrder.CreatedDate,
                    ModifiedDate = DateTime.UtcNow,
                    CreatedBy = userContext.UserId ?? 0,
                    ModifiedBy = userContext.UserId ?? 0
                });

                foreach (var recipe in menu.Recipes)
                {
                    double requiredQty = recipe.QuantityRequired * item.QuantityOrdered;
                    if (requiredIngredients.ContainsKey(recipe.ItemId))
                        requiredIngredients[recipe.ItemId] += requiredQty;
                    else
                        requiredIngredients.Add(recipe.ItemId, requiredQty);
                }
            }

            foreach (var kvp in requiredIngredients)
            {
                var inventory = await unitOfWork.InventoryRepository.GetById(kvp.Key);
                if (inventory.QuantityAvailable < kvp.Value)
                    return false;

                inventory.QuantityAvailable -= kvp.Value;
                inventory.ModifiedDate = DateTime.UtcNow;
                inventory.ModifiedBy = userContext.UserId ?? 0;
                unitOfWork.InventoryRepository.Update(inventory);
            }

            existingOrder.CustomerName = updatedOrderDto.CustomerName;
            existingOrder.TotalAmount = newTotal;
            existingOrder.OrderDate = DateTime.UtcNow;
            existingOrder.ModifiedDate = DateTime.UtcNow;
            existingOrder.ModifiedBy = userContext.UserId ?? 0;

            unitOfWork.OrderRepository.Update(existingOrder);
            var changes=await unitOfWork.SaveChangesAsync()>0;

            return changes;
        }
        public async Task<bool?> Delete(int id)
        {
            var order = await unitOfWork.OrderRepository.GetOrderWithDetailsAsync(id);
            if (order == null)
            {
                return false;
            }
            foreach(var o in order.OrderDetails)
            {
                var menu = await unitOfWork.MenuRepository.GetMenuWithRecipeAsync(o.MenuId);
                if (menu == null) continue;
                foreach(var m in menu.Recipes)
                {
                    var item = await unitOfWork.InventoryRepository.GetById(m.ItemId);
                    if (item == null) continue;
                    item.QuantityAvailable += m.QuantityRequired * o.QuantityOrdered;
                    item.ModifiedDate = DateTime.UtcNow;
                    item.ModifiedBy = userContext.UserId ?? 0;
                    unitOfWork.InventoryRepository.Update(item);
                }
            }
            unitOfWork.OrderRepository.Remove(order);
            var changes = await unitOfWork.SaveChangesAsync() > 0;
            return changes;
        }
        public async Task<IEnumerable<Order>?> GetDailyOrderReport(DateTime date)
        {
            return await unitOfWork.OrderRepository.GetDailyOrderReport(date);
        }
        public async Task<byte[]?> SendDailyReportByEmail(DateTime date, string? toEmail)
        {
            var orders = await unitOfWork.OrderRepository.GetDailyOrderReport(date);
            if (!orders.Any())
                return null;
            var reportData = orders.Select(o => new DailyOrderReportDto
            {
                OrderId = o.Id,
                CustomerName = o.CustomerName,
                OrderDate = o.OrderDate.ToLocalTime(),
                TotalAmount = o.TotalAmount,
                OrderDetails = o.OrderDetails.Select(d => new DailyOrderDetailDto
                {
                    MenuName = d.Menu!.MenuName,
                    QuantityOrdered = d.QuantityOrdered,
                    UnitPrice = d.Menu.Price
                }).ToList()
            }).ToList();

            var excelBytes = GenerateExcelReport(reportData);
            var body = $"Please find attached daily sales report for {date:yyyy-MM-dd}.";
            var subject = $"Daily Sales Report - {date:yyyy-MM-dd}";
            if (toEmail != null)
            {
                await emailService.SendEmail(toEmail,subject,body,excelBytes, "DailySales.xlsx", "vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            return excelBytes;
        }

        private byte[] GenerateExcelReport(IEnumerable<DailyOrderReportDto> report)
        {
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("DailySales");

            ws.Cells[1, 1].Value = "Order ID";
            ws.Cells[1, 2].Value = "Customer Name";
            ws.Cells[1, 3].Value = "Order Date";
            ws.Cells[1, 4].Value = "Menu Name";
            ws.Cells[1, 5].Value = "Quantity Ordered";
            ws.Cells[1, 6].Value = "Unit Price";
            ws.Cells[1, 7].Value = "Line Total";
            ws.Cells[1, 8].Value = "Order Total";

            using (var range = ws.Cells[1, 1, 1, 8])
            {
                range.Style.Font.Bold = true;
                range.Style.Font.Color.SetColor(Color.White);
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); 
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            }

            int row = 2;
            double t = 0;
            foreach (var order in report)
            {
                int startRow = row; 

                foreach (var detail in order.OrderDetails)
                {
                    ws.Cells[row, 4].Value = detail.MenuName;
                    ws.Cells[row, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[row, 5].Value = detail.QuantityOrdered;
                    ws.Cells[row, 6].Value = detail.UnitPrice;
                    ws.Cells[row, 7].Value = detail.LineTotal;
                    row++;
                }

                int endRow = row - 1;
                t += order.TotalAmount;
                ws.Cells[startRow, 1,endRow,1].Merge = true;
                ws.Cells[startRow, 1].Value = order.OrderId;
                ws.Cells[startRow, 2, endRow, 2].Merge = true;
                ws.Cells[startRow, 2].Value = order.CustomerName;
                ws.Cells[startRow, 3, endRow, 3].Merge = true;
                ws.Cells[startRow, 3].Value = order.OrderDate.ToString("yyy-MM-dd HH:mm");
                ws.Cells[startRow, 8, endRow, 8].Merge = true;
                ws.Cells[startRow, 8].Value = order.TotalAmount;
                ws.Cells[startRow, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Cells[startRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells[startRow, 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Cells[startRow, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells[startRow, 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Cells[startRow, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells[startRow, 8].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Cells[startRow, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }
            ws.Cells[row, 8].Value = $"Total={t}";
            ws.Cells[row, 8].Style.Font.Bold = true;
            ws.Cells[row, 8].Style.Font.Color.SetColor(Color.Black);
            ws.Cells[row, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, 8].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
            ws.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[row, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            using (var range = ws.Cells[1, 1, row - 1, 8])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Top.Color.SetColor(Color.Black);
                range.Style.Border.Left.Color.SetColor(Color.Black);
                range.Style.Border.Right.Color.SetColor(Color.Black);
                range.Style.Border.Bottom.Color.SetColor(Color.Black);
            }
            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            return package.GetAsByteArray();
        }

    }
}
