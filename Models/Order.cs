namespace TechParser.Models
{
    public enum OrderStatus
    {
        None = 0,
        Active = 1,
        Archive = 2
    }
    public class Order
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public string OrderNumber { get; set; }
        public string PublicationDate { get; set; }
        public string Material { get; set; }
        public OrderStatus Status { get; set; }
        public List<string> ProcessingTypes { get; set; }
        public string NameDetail { get; set; }
        public string Description { get; set; }
        public string Circulation { get; set; }
        public string Price { get; set; }
        public string ExpirationDate { get; set; }
        public string DownloadFileUrl { get; set; }

        //не заносится в базу
        public virtual Resource Resource { get; set; }
    }
}
