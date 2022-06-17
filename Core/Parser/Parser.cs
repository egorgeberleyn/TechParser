using HtmlAgilityPack.CssSelectors.NetCore;
using System.Text.RegularExpressions;
using TechParser.Core.Data;
using TechParser.Models;

namespace TechParser.Core.Parser;

public class Parser
{               
    public Parser(string url)
    {
        _parserSettings = new ParserSettings(url); //добавление настроек парсера
    }
    private readonly ParserSettings _parserSettings;      
    public void ParseProviders(ParserDbContext context)
    {        
        _parserSettings.EndPoint = 12; //последняя страница
        for (int i = _parserSettings.StartPoint; i <= _parserSettings.EndPoint; i++)
        {
            _parserSettings.Prefix = $"/company/{i}";
            var document = _parserSettings.GetHtmlDocument();
            var nodeCollection = document.QuerySelectorAll("div.company_card"); //карточки производителей

            foreach (var node in nodeCollection)
            {
                _parserSettings.Prefix = node.QuerySelector("a.com_cont").Attributes[0].Value; //ссылка для перехода на страницу с описанием
                var descriptionDocument = _parserSettings.GetHtmlDocument();
                var serviceList = descriptionDocument
                    .QuerySelectorAll("ul.serv-list")
                    .Select(node => Regex.Replace(node.InnerText, @"[\r\t\b]", ""))
                    .ToList();
                var identityNumber = descriptionDocument.QuerySelector("div.contact-card").ChildNodes[1].InnerText;
                var providerIdentityNumbers = context.Providers.Select(prov => prov.TaxpayerIdentificationNumber).ToList();
                
                if (providerIdentityNumbers.Contains(identityNumber))
                    continue;
                
                else context.Providers.Add(new Provider
                {
                    CompanyName = node.QuerySelector("h4>a").InnerText.Replace("&raquo;", " ").Replace("&laquo;", " "),
                    Adress = node.QuerySelector("p").InnerText,
                    CompanyDescription = Regex.Replace(node.QuerySelector("div.icons").InnerText, @"[\r\t\b]", " "),
                    TaxpayerIdentificationNumber = identityNumber,
                    TypesOfServices = serviceList,
                });

                context.SaveChanges();
            }                
        }        
    }


    public void ParseOrders(ParserDbContext context)
    {               
        _parserSettings.Prefix = $"/orders";
        var document = _parserSettings.GetHtmlDocument();
        var nodeCollection = document.QuerySelectorAll("div.col-md-8>p.category"); //карточки заказов
        
        foreach (var node in nodeCollection)
        {
            _parserSettings.Prefix = node.ParentNode.QuerySelector("a.btn-order").Attributes[0].Value;                
            var descriptionDocument = _parserSettings.GetHtmlDocument();
            
            List<string> orderInfo = node.ParentNode.QuerySelectorAll("strong").Select(el => el.InnerText).ToList();
            var dateAndNumber = node.ParentNode.QuerySelector("p.date").InnerText.Split("от", StringSplitOptions.RemoveEmptyEntries);
            List<string> processTypeList = node.QuerySelectorAll("span").Select(el => el.InnerText).ToList();

            var currentOrderNumber = dateAndNumber[0];
            var orderNumbers = context.Orders.Select(ord => ord.OrderNumber).ToList();

            if (orderNumbers.Contains(currentOrderNumber))
                continue;
            else context.Orders.Add(new Order
            {
                OrderNumber = currentOrderNumber,
                NameDetail = node.ParentNode.QuerySelector("div.col-md-8>h4>a").InnerText,
                PublicationDate = dateAndNumber[1].Replace("Открыт", ""),
                Circulation = orderInfo[0],
                Price = orderInfo[1],
                ExpirationDate = orderInfo[2],
                ProcessingTypes = processTypeList,
                Description = descriptionDocument.QuerySelector("div.card-text>p:last-child").InnerText
                        .Replace("&mdash;", "—")
                        .Replace("&hellip;", "...")
                        .Replace("&nbsp;", " "),
                ResourceId = context.Resources.Where(res => res.Name == "Металлообработчики").First().Id
            });

            context.SaveChanges();            
        }       
    }

    public void ParseResources(ParserDbContext context)
    {
        if (context.Resources.Count() == 2)
            return;
        
        context.Resources.Add(new Resource 
        {
            Adress = "partnerzakaz.ru",
            Description = "Заказы на металлообработку, металлоконструкции, технологическую оснастку и готовые изделия. Россия и СНГ",
            Name = "PartnerZakaz" 
        });

        context.Resources.Add(new Resource
        {
            Adress = "metalloobrabotchiki.ru",
            Description = "Портал «Металлообработчики» в Челябинской области",
            Name = "Металлообработчики"
        });
        
        context.SaveChanges();
    }
}
