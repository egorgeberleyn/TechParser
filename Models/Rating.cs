namespace TechParser.Models
{
    public class Rating
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid ProviderId { get; set; }
        public double RatingCompany { get; set; }
        public int CountOfRatings { get; set; }

        //не заносить в базу 
        public ICollection<Client> Clients { get; set; }
        public Provider Provider { get; set; }
    }
}
