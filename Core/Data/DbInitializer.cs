namespace TechParser.Core.Data
{
    public class DbInitializer
    {
        public static void Initialize(ParserDbContext context) //инициализация бд на основе контекста
        {
            //если бд не создана, создается новая
            {
                var parser = new Parser.Parser(@"https://metalloobrabotchiki.ru");
                parser.ParseResources(context);
                parser.ParseProviders(context);
                parser.ParseOrders(context);
            }
        }
    }
}
