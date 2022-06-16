using TechParser.Core.Parser;

namespace TechParser.Core.Data
{
    public class DbInitializer
    {
        public static void Initialize(ParserDbContext context) //инициализация бд на основе контекста
        {
            if (context.Database.EnsureCreated())//если бд не создана, создается новая
            {
                var parser = new Parser.Parser(@"https://metalloobrabotchiki.ru");
                await parser.ParseProviders(context);
                await parser.ParseOrders(context);
            }
        }
    }
}
