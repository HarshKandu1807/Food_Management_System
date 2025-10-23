using Food_Management_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.DTOS
{
    public class RecipeDto
    {
        public int MenuId { get; set; }
        public int ItemId { get; set; }
        public int QuantityRequired { get; set; }
    }
}
