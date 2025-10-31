using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Food_Management_System.Domain.Entities
{
    public class Order:BaseEntity
    {
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        [JsonIgnore]
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
