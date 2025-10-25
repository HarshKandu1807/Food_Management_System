using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.DTOS
{
    public class LoginDto
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int ContactNo { get; set; }
    }
}
