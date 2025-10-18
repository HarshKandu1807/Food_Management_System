namespace Food_Management_System.Domain.Entities
{
    public class OrderDetail:BaseEntity
    {
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int MenuId { get; set; }
        public Menu? Menu { get; set; }
        public int QuantityOrdered { get; set; }
    }
}
