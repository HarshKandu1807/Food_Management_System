using Food_Management_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.DTOS
{
    public class OrderDetailDto
    {
        public int MenuId { get; set; }
        public int QuantityOrdered { get; set; }
    }
}
