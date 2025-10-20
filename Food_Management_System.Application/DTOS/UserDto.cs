using Food_Management_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Food_Management_System.Application.DTOS
{
    public class UserDto
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int ContactNo { get; set; }
        public string Role { get; set; }
    }
}
