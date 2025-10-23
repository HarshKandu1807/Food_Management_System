using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Domain.Entities;
using Food_Management_System.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.UserService
{
    public class UserService:IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public UserService(IUnitOfWork _unitOfWork,IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public async Task<IEnumerable<User>?> GetAll()
        {
            return await unitOfWork.UserRepository.GetAll();
        }
        public async Task<User?> GetById(int id)
        {
            return await unitOfWork.UserRepository.GetById(id);
        }
        public async Task<bool> Create(UserDto userDto)
        {
            var user = mapper.Map<User>(userDto);
            user.CreatedDate = DateTime.UtcNow;
            user.ModifiedDate = DateTime.UtcNow;
            await unitOfWork.UserRepository.Add(user);
            var changes = await unitOfWork.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<bool?> Update(int id,UserDto userDto)
        {
            var user = await unitOfWork.UserRepository.GetById(id);
            if (user == null)
            {
                return null;
            }
            user.ModifiedDate = DateTime.UtcNow;
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
