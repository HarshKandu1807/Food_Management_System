using System.Text.Json.Serialization;

namespace Food_Management_System.Domain.Entities
{
    public class Menu:BaseEntity
    {
        public string MenuName { get; set; }
        public int Price { get; set; }
        [JsonIgnore]
        public ICollection<Recipe>? Recipes { get; set; }
    }
}
