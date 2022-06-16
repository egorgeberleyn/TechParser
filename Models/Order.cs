namespace TechParser.Models
{
    public class Order
    {
        public int Id { get; set; }
        //public Guid ResourceId { get; set; }
        public string OrderNumber { get; set; }
        public string PublicationDate { get; set; }
        public string Material { get; set; }
        public List<string> ProcessingTypes { get; set; }
        public string NameDetail { get; set; }
        public string Description { get; set; }
        public string Circulation { get; set; }
        public string Price { get; set; }
        public string ExpirationDate { get; set; }

        //не заносится в базу
        //public Resource Resource { get; set; }
    }
}
