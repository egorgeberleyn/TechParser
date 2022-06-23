using HtmlAgilityPack.CssSelectors.NetCore;
using System.Text.RegularExpressions;
using TechParser.Core.Data;
using TechParser.Models;

namespace TechParser.Core.Parser;

public class ParserMetalloobrabotchiki
{               
    public ParserMetalloobrabotchiki(string url, ParserDbContext dbcontext)
    {
        parserSettings = new ParserSettings(url); //добавление настроек парсера
        context = dbcontext;                                          
    }
    private readonly ParserSettings parserSettings;
    private readonly ParserDbContext context;    
    public void ParseProviders()
    {
        var providerIdentityNumbers = context.Providers.Select(prov => prov.TaxpayerIdentificationNumber).ToList();
                
        var mainDocument = parserSettings.GetHtmlDocument();
        var regions = mainDocument.QuerySelector("ul").QuerySelectorAll("a").Select(reg => reg.Attributes[0].Value);

        foreach (var item in regions)
        {
            parserSettings.BaseUrl = item;            
            parserSettings.Prefix = $"/company/1";
                        
            try
            {
                parserSettings.EndPoint = Convert.ToInt32(parserSettings.GetHtmlDocument().QuerySelector("")); //последняя страница 
            }
            catch (Exception)
            {
                parserSettings.EndPoint = 1;
            }
                   
            for (int i = parserSettings.StartPoint; i <= parserSettings.EndPoint; i++)
            {
                parserSettings.Prefix = $"/company/{i}";
                var document = parserSettings.GetHtmlDocument();
                var nodeCollection = document.QuerySelectorAll("div.company_card"); //карточки производителей

                foreach (var node in nodeCollection)
                {
                    parserSettings.Prefix = node.QuerySelector("a.com_cont").Attributes[0].Value; //ссылка для перехода на страницу с описанием
                    var descriptionDocument = parserSettings.GetHtmlDocument();

                    var identityNumber = descriptionDocument.QuerySelector("div.contact-card").ChildNodes[1].InnerText;
                    if (providerIdentityNumbers.Contains(identityNumber))
                        continue;

                    var serviceList = descriptionDocument
                        .QuerySelectorAll("ul.serv-list")
                        .Select(node => Regex.Replace(node.InnerText, @"[\r\t\b]", "").Trim())
                        .ToList();

                    context.Providers.Add(new Provider
                    {
                        CompanyName = node.QuerySelector("h4>a").InnerText.Replace("&raquo;", " ").Replace("&laquo;", " "),
                        Adress = node.QuerySelector("p").InnerText,
                        CompanyDescription = Regex.Replace(node.QuerySelector("div.icons").InnerText, @"[\r\t\b]", " ").Trim(),
                        TaxpayerIdentificationNumber = identityNumber,
                        TypesOfServices = serviceList,
                    });
                }                
            }
        }
        context.SaveChanges();        
    }

    public void ParseOrders()
    {
        var orderNumbers = context.Orders
            .Where(ord => ord.ResourceId == 5)
            .Select(ord => ord.OrderNumber)
            .ToList(); //достаем текущий набор карточек заказов из базы

        parserSettings.Prefix = $"/orders";
        var document = parserSettings.GetHtmlDocument();
        var nodeCollection = document.QuerySelectorAll("div.col-md-8>p.category"); //карточки заказов

        var resourceID = context.Resources.Where(res => res.Name == "Металлообработчики").First().Id;
        foreach (var node in nodeCollection)
        {                        
            //проверка заказа в базе
            var dateAndNumber = node.ParentNode.QuerySelector("p.date").InnerText.Split("от", StringSplitOptions.RemoveEmptyEntries);
            var currentOrderNumber = dateAndNumber[0];
            if (orderNumbers.Contains(currentOrderNumber))
                continue;
            
            //получение страницы с подробностями
            parserSettings.Prefix = node.ParentNode.QuerySelector("a.btn-order").Attributes[0].Value;
            var descriptionDocument = parserSettings.GetHtmlDocument();

            List<string> orderInfo = node.ParentNode.QuerySelectorAll("strong").Select(el => el.InnerText).ToList();            
            List<string> processTypeList = node.QuerySelectorAll("span").Select(el => el.InnerText).ToList();

            var downloadFileUrl = string.Empty;
            if(descriptionDocument.QuerySelector("body").InnerHtml.Contains("class=\"dwn_files\""))
            {
                downloadFileUrl = $"{parserSettings.BaseUrl}{descriptionDocument.QuerySelector("a.dwn_files").Attributes[0].Value}";
            }

            context.Orders.Add(new Order
            {
                OrderNumber = currentOrderNumber,
                NameDetail = node.ParentNode.QuerySelector("div.col-md-8>h4>a").InnerText,
                PublicationDate = dateAndNumber[1].Replace("Открыт", "").Trim(),
                Circulation = orderInfo[0],
                Price = orderInfo[1],
                ExpirationDate = orderInfo[2],
                ProcessingTypes = processTypeList,
                Description = descriptionDocument.QuerySelector("div.card-text>p:last-child").InnerText
                        .Replace("&mdash;", "—")
                        .Replace("&hellip;", "...")
                        .Replace("&nbsp;", " ").Trim(),
                ResourceId = resourceID,
                Status = OrderStatus.Active,
                DownloadFileUrl = downloadFileUrl
            });                       
        }
        context.SaveChanges();       
    }   
}
