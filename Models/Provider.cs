namespace TechParser.Models
{
    public class Provider
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public string CompanyName { get; set; }
        public string Adress { get; set; }
        public string CompanyDescription { get; set; }
        public string TaxpayerIdentificationNumber { get; set; }
        public List<string> TypesOfServices { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
    }
}
