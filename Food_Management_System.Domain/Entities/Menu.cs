namespace Food_Management_System.Domain.Entities
{
    public class Menu:BaseEntity
    {
        public string MenuName { get; set; }
        public int Price { get; set; }
        public ICollection<Recipe>? Recipes { get; set; }
    }
}
