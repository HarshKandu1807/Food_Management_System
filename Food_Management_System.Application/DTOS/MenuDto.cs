using Food_Management_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Food_Management_System.Application.DTOS
{
    public class MenuDto
    {
        public string MenuName { get; set; }
        public double Price { get; set; }
        //public ICollection<RecipeDto>? Recipes { get; set; }
    }
}
