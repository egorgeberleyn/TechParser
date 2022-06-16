using Microsoft.EntityFrameworkCore;
using TechParser.Models;

namespace TechParser.Core.Data
{
    public class ParserDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }
        public DbSet<ParseFile> ParseFiles { get; set; }

        public ParserDbContext(DbContextOptions<ParserDbContext> options) : base(options) { }
               
    }
}
