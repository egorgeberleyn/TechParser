using TechParser.Core.Data;
using TechParser.Models;

namespace TechParser.Core.Parser
{
    public class ResourcesParser
    {
        public static void ParseResources(ParserDbContext context)
        {
            if (context.Resources.Count() == 6)
                return;

            context.Resources.Add(new Resource
            {
                Adress = "metallportal.com",
                Description = "Тут можно разместить любой заказ на обработку и изготовление изделий, металлопрокат и снабжение производства",
                Name = "MetallPortal"
            });

            context.Resources.Add(new Resource
            {
                Adress = "obrabotka.net",
                Description = "Эффективный инструмент для привлечения и поиска заказов для вашего металлообрабатывающего предприятия",
                Name = "obrabotka.net"
            });

            context.Resources.Add(new Resource
            {
                Adress = "partnerzakaz.ru",
                Description = "Заказы на металлообработку, металлоконструкции, технологическую оснастку и готовые изделия. Россия и СНГ",
                Name = "PartnerZakaz"
            });

            context.Resources.Add(new Resource
            {
                Adress = "www.iprom.ru",
                Description = "Помогает решить главную проблему развития промышленных предприятий – найти новых заказчиков, " +
                "увеличить спрос на продукцию и услуги предприятий, а также значительно сократить время поиска поставщиков промышленной продукции.",
                Name = "IProm"
            });

            context.Resources.Add(new Resource
            {
                Adress = "metalloobrabotchiki.ru",
                Description = "Портал «Металлообработчики» в Челябинской области",
                Name = "Металлообработчики"
            });

            context.Resources.Add(new Resource
            {
                Adress = "prom-market.com",
                Description = "Это площадка подбора исполнителей, поиска заказов по обработке металла и интернет магазин инструмента и оснастки",
                Name = "ПромМаркет"
            });



            context.SaveChanges();
        }
    }
}
