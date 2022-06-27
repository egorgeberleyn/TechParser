using HtmlAgilityPack.CssSelectors.NetCore;
using System.Text.RegularExpressions;
using TechParser.Core.Data;
using TechParser.Models;
using TechParser.Storage;

namespace TechParser.Core.Parser;

public class ParserMetalloobrabotchiki
{               
    public ParserMetalloobrabotchiki(string url, IStorage storage)
    {
        parserSettings = new ParserSettings(url); //добавление настроек парсера
        _storage = storage;
    }
    
    private readonly ParserSettings parserSettings;
    private readonly IStorage _storage;  
    
    public void ParseProviders()
    {
        int resId = _storage.GetResourceId("Металлообработчики");
        var contextProviders = _storage.GetContextProviders(resId);
        
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

                    var companyName = node.QuerySelector("h4>a").InnerText.Replace("&raquo;", " ")
                        .Replace("&laquo;", " ");
                    if (contextProviders.Contains(companyName))
                        continue;

                    var serviceList = descriptionDocument
                        .QuerySelectorAll("ul.serv-list")
                        .Select(node => Regex.Replace(node.InnerText, @"[\r\t\b]", "").Trim())
                        .ToList();

                    _storage.AddProvider(new Provider
                    {
                        CompanyName = companyName,
                        ResourceId = resId,
                        Adress = node.QuerySelector("p").InnerText,
                        CompanyDescription = Regex.Replace(node.QuerySelector("div.icons").InnerText, @"[\r\t\b]", " ").Trim(),
                        TaxpayerIdentificationNumber =  descriptionDocument.QuerySelector("div.contact-card").ChildNodes[1].InnerText,
                        TypesOfServices = serviceList,
                    });
                }                
            }
        }
    }

    public void ParseOrders()
    {
        var resId = _storage.GetResourceId("Металлообработчики");
        var contextOrders = _storage.GetContextOrders(resId);

        parserSettings.Prefix = $"/orders";
        var document = parserSettings.GetHtmlDocument();
        var nodeCollection = document.QuerySelectorAll("div.col-md-8>p.category"); //карточки заказов
        
        foreach (var node in nodeCollection)
        {                        
            //проверка заказа в базе
            var dateAndNumber = node.ParentNode.QuerySelector("p.date").InnerText.Split("от", StringSplitOptions.RemoveEmptyEntries);
            var currentOrderNumber = dateAndNumber[0];
            if (contextOrders.Contains(currentOrderNumber))
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

            _storage.AddOrder(new Order
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
                ResourceId = resId,
                Status = OrderStatus.Active,
                DownloadFileUrl = downloadFileUrl
            });                       
        }
    }   
}
