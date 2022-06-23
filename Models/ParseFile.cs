namespace TechParser.Models
{
    public class ParseFile
    {
        public int Id { get; set; }
        public string NameFile { get; set; }       
        public int OrderId { get; set; }

        //не в базу
        public virtual Order Order { get; set; }
    }
}
