using System.ComponentModel.DataAnnotations;

namespace Food_Management_System.Domain.Entities
{
    public class User:BaseEntity
    {
        public string Name { get; set; }
        public string Password { get; set; }
        [MaxLength(10)]
        public int ContactNo { get; set; }
        public string Role { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
