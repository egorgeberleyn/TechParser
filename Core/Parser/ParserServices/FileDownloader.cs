using TechParser.Core.Data;
using TechParser.Models;

namespace TechParser.Core.Parser
{
    public class FileDownloader
    {
        private readonly ParserDbContext _context;
        public FileDownloader(ParserDbContext context)
        {
            _context = context;
        }

        public async void DownloadFilesObrNet()
        {
            var orders = _context.Orders.Where(ord => ord.ResourceId == 2).ToList();
            byte[] data;
            using var client = new HttpClient();
            var metalloobrabotchikiFiles = CreateDirectory(@"C:\Users\Tema\Desktop\ObrabotkaNetFiles");

            foreach (var order in orders)
            {
                if (order.DownloadFileUrl == string.Empty)
                    continue;
                using HttpResponseMessage response = await client.GetAsync(order.DownloadFileUrl);
                using HttpContent content = response.Content;
                data = await content.ReadAsByteArrayAsync();
                using FileStream file = File.Create($@"C:\Users\Tema\Desktop\ObrabotkaNetFiles\{order.OrderNumber}.zip"); //path = "wwwroot\XML\1.zip"
                file.Write(data, 0, data.Length);
                _context.ParseFiles.Add(new ParseFile
                {
                    DownloadUrl = order.DownloadFileUrl,
                    NameFile = order.OrderNumber,
                    OrderId = order.Id
                });
            }
            _context.SaveChanges();
        }

        public async void DownloadFilesMetObr()
        {
            var orders = _context.Orders.Where(ord => ord.ResourceId == 5).ToList();
            byte[] data;
            using var client = new HttpClient();
            var metalloobrabotchikiFiles = CreateDirectory(@"C:\Users\Tema\Desktop\MetalloobrabotchikiFiles");

            foreach (var order in orders)
            {
                if (order.DownloadFileUrl == string.Empty) 
                    continue;
                using HttpResponseMessage response = await client.GetAsync(order.DownloadFileUrl);
                using HttpContent content = response.Content;
                data = await content.ReadAsByteArrayAsync();
                using FileStream file = File.Create($@"C:\Users\Tema\Desktop\MetalloobrabotchikiFiles\{order.OrderNumber}.zip"); //path = "Desktop\папка\архивЗаказов.zip"
                file.Write(data, 0, data.Length);
                _context.ParseFiles.Add(new ParseFile
                {
                    DownloadUrl = order.DownloadFileUrl,
                    NameFile = order.OrderNumber,
                    OrderId = order.Id
                });
            }
            _context.SaveChanges();
        }

        public async void DownloadFilesMetallPortal()
        {
            var orders = _context.Orders.Where(ord => ord.ResourceId == 1).ToList();
            byte[] data;
            using var client = new HttpClient();
            var mettallPortalFiles = CreateDirectory(@"C:\Users\Tema\Desktop\MetallPortalFiles");
            foreach (var order in orders)
            {
                if (order.DownloadFileUrl != string.Empty)
                {
                    using HttpResponseMessage response = await client.GetAsync(order.DownloadFileUrl);
                    using HttpContent content = response.Content;
                    data = await content.ReadAsByteArrayAsync();
                    using FileStream file = File.Create($@"C:\Users\Tema\Desktop\MetallPortalFiles\{order.OrderNumber}.zip");
                    file.Write(data, 0, data.Length);
                    _context.ParseFiles.Add(new ParseFile
                    {
                        DownloadUrl = order.DownloadFileUrl,
                        NameFile = order.OrderNumber,
                        OrderId = order.Id
                    });
                }

                _context.SaveChanges();
            }
        }

        public static DirectoryInfo CreateDirectory(string path) //создание папки под заказы
        {
            DirectoryInfo directory = new(path);
            if (!directory.Exists)
                directory.Create();
            return directory;
        }

    }        
    
}
