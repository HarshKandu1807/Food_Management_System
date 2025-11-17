using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services.Helper;
using Food_Management_System.Domain.Entities;
using Food_Management_System.Domain.Interfaces;
using Food_Management_System.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.UserService
{
    public class UserService:IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IUserContext userContext;
        public UserService(IUnitOfWork _unitOfWork,IMapper _mapper,IConfiguration _configuration,IUserContext _userContext)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            configuration = _configuration;
            userContext = _userContext;
        }
        public async Task<Pagination<User>?> GetAll(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allItems = await unitOfWork.UserRepository.GetAll();
                return new Pagination<User>
                {
                    Items = allItems,
                    PageNumber = 1,
                    PageSize = allItems.Count,
                    TotalCount = allItems.Count
                };
            }
            return await unitOfWork.UserRepository.GetAll(pageNumber, pageSize);
        }
        public async Task<User?> GetById(int id)
        {
            return await unitOfWork.UserRepository.GetById(id);
        }
        public async Task<string> Create(UserDto dto)
        {
            var existingUser = await unitOfWork.UserRepository.FindByContact(dto.ContactNo);

            if (existingUser != null)
                throw new Exception("Username already exists.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Name = dto.Name,
                Password = hashedPassword,
                Role = dto.Role,
                ContactNo = dto.ContactNo,
                CreatedBy = userContext.UserId ?? 0,
                ModifiedBy=userContext.UserId??0
            };

            await unitOfWork.UserRepository.Add(user);
            await unitOfWork.SaveChangesAsync();

            return "User registered successfully.";
        }
        public async Task<bool?> Update(int id,UserDto userDto)
        {
            var user = await unitOfWork.UserRepository.GetById(id);
            if (user == null)
            {
                return null;
            }
            user.ModifiedDate = DateTime.UtcNow;
            user.ModifiedBy = userContext.UserId ?? 0;
            mapper.Map(userDto,user);
            unitOfWork.UserRepository.Update(user);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<bool?> Delete(int id)
        {
            var user = await unitOfWork.UserRepository.GetById(id);
            if (user == null)
            {
                return null;
            }
            unitOfWork.UserRepository.Remove(user);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
    }
}
