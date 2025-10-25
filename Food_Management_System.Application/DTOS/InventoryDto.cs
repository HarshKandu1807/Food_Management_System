using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.DTOS
{
    public class InventoryDto
    {
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public double QuantityAvailable { get; set; }
        public int ReorderLevel { get; set; }
    }
}
