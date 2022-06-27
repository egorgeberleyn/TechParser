using Microsoft.EntityFrameworkCore;
using TechParser.Core.Data;
using TechParser.Models;

namespace TechParser.Storage
{

    public interface IStorage
    {
        void AddOrder(Order order);
        List<string> GetContextOrders(int resId);
        void AddProvider(Provider provider);
        List<string> GetContextProviders(int resId);
        int GetResourceId(string resName);
        List<string> GetActiveOrders(int resId);
        List<string> GetArchiveOrders(int  resId);
        void AddSuggestions(Suggestion suggestion);
        List<Order> GetOrdersMetallPortal();

    }

    public class Storage : IStorage
    {
        public Storage(ParserDbContext context)
        {
            _parserDbContext = context;
        }

        public ParserDbContext _parserDbContext { get; }

        public void AddOrder(Order order)
        {
            _parserDbContext.Orders.Add(order);
            _parserDbContext.SaveChanges();
        }

        public List<string> GetContextOrders(int resId) => _parserDbContext.Orders
            .Where(order => order.ResourceId == resId)
            .Select(order => order.OrderNumber).ToList();

        public void AddProvider(Provider provider)
        {
            _parserDbContext.Providers.Add(provider);
            _parserDbContext.SaveChanges();
        }

        public List<string> GetContextProviders(int resId) => _parserDbContext.Providers
            .Where(prov => prov.ResourceId == resId)
            .Select(prov => prov.CompanyName).ToList();

        public int GetResourceId(string resName) =>  _parserDbContext.Resources.Where(res => res.Name == resName).First().Id;
        
        public List<string> GetActiveOrders(int resId) => _parserDbContext.Orders
            .Where(ord => ord.ResourceId == resId && ord.Status == OrderStatus.Active)
            .Select(ord => ord.OrderNumber)
            .ToList(); 
        
        public List<string> GetArchiveOrders(int resId) => _parserDbContext.Orders
            .Where(ord => ord.ResourceId == resId && ord.Status == OrderStatus.Archive)
            .Select(ord => ord.OrderNumber)
            .ToList();

        public void AddSuggestions(Suggestion suggestion)
        {
            _parserDbContext.Suggestions.Add(suggestion);
            _parserDbContext.SaveChanges();
        }

        public List<Order> GetOrdersMetallPortal() => _parserDbContext.Orders
            .Where(ord => ord.ResourceId == 1)
            .Select(ord => ord).ToList();


    }
}

