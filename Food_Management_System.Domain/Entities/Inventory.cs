using System.ComponentModel.DataAnnotations;

namespace Food_Management_System.Domain.Entities
{
    public class Inventory:BaseEntity
    {
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public double QuantityAvailable { get; set; }
        public int ReorderLevel { get; set; }
        public ICollection<Recipe>? Recipes { get; set; }
    }
}
