namespace TechParser.Models
{
    public class Suggestion
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProviderId { get; set; }
        public DateTime Term { get; set; }
        public decimal Price { get; set; }
        public string Comment { get; set; }

        //не в базу
        public Order Order { get; set; }
        public Provider Provider { get; set; }
    }
}
