using AutoMapper;
using Food_Management_System.Application.DTOS;
using Food_Management_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile(){
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Menu, MenuDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<Inventory, InventoryDto>().ReverseMap();
            CreateMap<Recipe, RecipeDto>().ReverseMap();

        }
    }
}
