using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Food_Management_System.Domain.Entities
{
    public class Recipe:BaseEntity
    {
        public int MenuId { get; set; }
        public Menu? Menu { get; set; }
        [ForeignKey(nameof(Inventory))]
        public int ItemId { get; set; }
        public Inventory? Inventory { get; set; }
        public int QuantityRequired { get; set; }
    }
}
