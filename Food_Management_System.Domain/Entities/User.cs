using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Food_Management_System.Domain.Entities
{
    public class User:BaseEntity
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int ContactNo { get; set; }
        public UserRole Role { get; set; }
        [JsonIgnore]
        public ICollection<Order>? Orders { get; set; }
    }
    public enum UserRole
    {
        Admin,
        Cashier 
    }
}
