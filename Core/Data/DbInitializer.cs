using TechParser.Controllers;
using TechParser.Core.Parser;

namespace TechParser.Core.Data
{
    public class DbInitializer
    {
        public static void Initialize(ParserDbContext context) //инициализация бд на основе контекста
        { 
            //если бд не создана, создается новая
            {
                // storage
                var storage = new Storage.Storage(context);
                
                // parsers
                var parserMet = new ParserMetalloobrabotchiki(@"https://metalloobrabotchiki.ru", storage);
                var parserObr = new ParserObrabotkaNet(@"https://obrabotka.net/", storage);
                var parserPortal = new MetallPortalParser("https://metallportal.com/", storage);
                var parserProm = new ParserPromMarket("https://prom-market.com", storage);
                // controllers 

                //file downloader
                var fileDownloader = new FileDownloader(context);
                
                ResourcesParser.ParseResources(context); //заполнение базы ресурсов
                
                //Парсинг МеталлПортал
                parserPortal.ParseProvider();
                parserPortal.ParseOrder();

                //Парсинг металлообработчики                
                parserMet.ParseProviders();
                parserMet.ParseOrders();

                //Парсинг obrabotka.net                                              
                parserObr.ParseProviders();
                parserObr.ParseActiveOrders();
                parserObr.ParseArchiveOrders();
                
                //Парсинг prom-market
                parserProm.ParseProviders();

                //Скачивание файлов
                fileDownloader.DownloadFilesObrNet();
                fileDownloader.DownloadFilesMetObr();
                fileDownloader.DownloadFilesMetallPortal();
            }                       
        }
    }
}
