using TechParser.Core.Parser;

namespace TechParser.Core.Data
{
    public class DbInitializer
    {
        public static void Initialize(ParserDbContext context) //инициализация бд на основе контекста
        {            
            if (context.Database.EnsureCreated()) //если бд не создана, создается новая
            {
                var parserMet = new ParserMetalloobrabotchiki(@"https://metalloobrabotchiki.ru", context);
                var parserObr = new ParserObrabotkaNet(@"https://obrabotka.net/", context);
                var fileDownloader = new FileDownloader(context);
                
                ResourcesParser.ParseResources(context); //заполнение базы ресурсов

                //Парсинг металлообработчики                
                parserMet.ParseProviders();
                parserMet.ParseOrders();

                //Парсинг obrabotka.net                                              
                parserObr.ParseProviders();
                parserObr.ParseActiveOrders();
                parserObr.ParseArchiveOrders();

                //Скачивание файлов
                fileDownloader.DownloadFilesObrNet();
                fileDownloader.DownloadFilesMetObr();
            }                       
        }
    }
}
