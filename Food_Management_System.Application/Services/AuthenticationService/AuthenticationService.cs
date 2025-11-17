using Food_Management_System.Application.DTOS;
using Food_Management_System.Application.Services.Helper;
using Food_Management_System.Domain.Entities;
using Food_Management_System.Domain.Interfaces;
using Food_Management_System.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.AuthenticationService
{
    public class AuthenticationService:IAuthenticationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;
        private readonly IUserContext userContext;
        public AuthenticationService(IUnitOfWork _unitOfWork, IConfiguration _configuration, IUserContext _userContext)
        {
            unitOfWork = _unitOfWork;
            configuration = _configuration;
            userContext = _userContext;
        }
        public async Task<(string,string)> Login(LoginDto dto)
        {
            var user = await unitOfWork.UserRepository.FindByContact(dto.ContactNo);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                throw new Exception("Invalid username or password.");

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();
            if(refreshToken != null)
            {
                var token = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    IsActive = true,
                    CreatedBy = userContext.UserId ?? 0,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = userContext.UserId ?? 0,
                    ModifiedDate=DateTime.UtcNow
                };
                await unitOfWork.RefreshTokenRepository.Add(token);
                await unitOfWork.SaveChangesAsync();
            }
            return (accessToken, refreshToken);
        }
        public string GenerateAccessToken(User user)
        {
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

        public async Task<(string,string)> RevokeAsync(string token)
        {
            var tokenEntity = await unitOfWork.RefreshTokenRepository.GetByTokenAsync(token);

            if (tokenEntity == null)
                throw new Exception("Token not found.");

            tokenEntity.IsActive = false;
            tokenEntity.ModifiedBy = userContext.UserId ?? 0;
            tokenEntity.ModifiedDate = DateTime.UtcNow;

            var user = await unitOfWork.UserRepository.GetById(tokenEntity.UserId);
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();
            var tokenData = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsActive = true,
                CreatedBy = userContext.UserId ?? 0,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = userContext.UserId ?? 0,
                ModifiedDate = DateTime.UtcNow
            };
            await unitOfWork.RefreshTokenRepository.Add(tokenData);
            await unitOfWork.SaveChangesAsync();
            return (accessToken, refreshToken);
        }
        public async Task ExpireToken(string token)
        {
            var tokenEntity = await unitOfWork.RefreshTokenRepository.GetByTokenAsync(token);

            if (tokenEntity == null)
                throw new Exception("Token not found.");

            tokenEntity.IsActive = false;
            tokenEntity.ModifiedBy = userContext.UserId ?? 0;
            tokenEntity.ModifiedDate = DateTime.UtcNow;
            await unitOfWork.SaveChangesAsync();
        }
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

    }
}
