namespace TechParser.Models
{
    public class Suggestion
    {
        public int Id { get; set; }
        public int OrderId { get; set; }        
        public string Time { get; set; }
        public string Price { get; set; }
        public string Comment { get; set; }
        public string ContactInfo { get; set; }       
        
        //не в базу
        public virtual Order Order { get; set; }        
    }
}
