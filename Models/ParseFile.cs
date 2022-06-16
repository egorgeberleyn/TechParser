namespace TechParser.Models
{
    public class ParseFile
    {
        public Guid Id { get; set; }
        public string NameFile { get; set; }       
        public Guid OrderId { get; set; }

        //не в базу
        public Order Order { get; set; }
    }
}
