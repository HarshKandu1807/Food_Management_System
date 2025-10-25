using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.DTOS
{
    public class DailyOrderDetailDto
    {
        public string MenuName { get; set; }
        public int QuantityOrdered { get; set; }
        public double UnitPrice { get; set; }
        public double LineTotal => QuantityOrdered * UnitPrice;
    }
}
