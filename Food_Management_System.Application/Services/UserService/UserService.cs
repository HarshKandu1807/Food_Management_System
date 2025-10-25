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
        public async Task<IEnumerable<User>?> GetAll()
        {
            return await unitOfWork.UserRepository.GetAll();
        }
        public async Task<User?> GetById(int id)
        {
            return await unitOfWork.UserRepository.GetById(id);
        }
        //public async Task<bool> Create(UserDto userDto)
        //{
        //    var user = mapper.Map<User>(userDto);
        //    user.CreatedDate = DateTime.UtcNow;
        //    user.ModifiedDate = DateTime.UtcNow;
        //    await unitOfWork.UserRepository.Add(user);
        //    var changes = await unitOfWork.SaveChangesAsync();
        //    return changes > 0;
        //}
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

        public async Task<string> Login(LoginDto dto)
        {
            var user = await unitOfWork.UserRepository.FindByContact(dto.ContactNo);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                throw new Exception("Invalid username or password.");

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = creds,
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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
