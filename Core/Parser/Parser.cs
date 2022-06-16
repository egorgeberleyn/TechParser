using HtmlAgilityPack.CssSelectors.NetCore;
using System.Text.RegularExpressions;
using TechParser.Core.Data;
using TechParser.Models;

namespace TechParser.Core.Parser;

public class Parser
{               
    public Parser(string url)
    {
        _parserSettings = new ParserSettings(url);
    }
    private ParserSettings _parserSettings;      
    public async Task ParseProviders(ParserDbContext context)
    {
        var providers = new List<Provider>();
        _parserSettings.EndPoint = 12; //последняя страница
        for (int i = _parserSettings.StartPoint; i <= _parserSettings.EndPoint; i++)
        {
            _parserSettings.Prefix = $"/company/{i}";
            var document = _parserSettings.GetHtmlDocument();
            var nodeCollection = document.QuerySelectorAll("div.company_card");

            foreach (var node in nodeCollection)
            {
                _parserSettings.Prefix = node.QuerySelector("a.com_cont").Attributes[0].Value;
                var descriptionDocument = _parserSettings.GetHtmlDocument();
                var serviceList = descriptionDocument
                    .QuerySelectorAll("ul.serv-list")
                    .Select(node => Regex.Replace(node.InnerText, @"[\r\t\b]", ""))
                    .ToList();
                await context.Providers.AddAsync(
                    new Provider
                    {
                        CompanyName = node.QuerySelector("h4>a").InnerText.Replace("&raquo;", " ").Replace("&laquo;", " "),
                        Adress = node.QuerySelector("p").InnerText,
                        CompanyDescription = Regex.Replace(node.QuerySelector("div.icons").InnerText, @"[\r\t\b]", " "),
                        TaxpayerIdentificationNumber = descriptionDocument.QuerySelector("div.contact-card").ChildNodes[1].InnerText,
                        TypesOfServices = serviceList,
                    });
                await context.SaveChangesAsync();
            }                
        }        
    }


    public async Task ParseOrders(ParserDbContext context)
    {
        var orders = new List<Order>();
        _parserSettings.Prefix = $"/orders";
        var document = _parserSettings.GetHtmlDocument();
        var nodeCollection = document.QuerySelectorAll("div.col-md-8>p.category");
        
        foreach (var node in nodeCollection)
        {
            _parserSettings.Prefix = node.ParentNode.QuerySelector("a.btn-order").Attributes[0].Value;                
            var descriptionDocument = _parserSettings.GetHtmlDocument();
            
            List<string> orderInfo = node.ParentNode.QuerySelectorAll("strong").Select(el => el.InnerText).ToList();
            var dateAndNumber = node.ParentNode.QuerySelector("p.date").InnerText.Split("от", StringSplitOptions.RemoveEmptyEntries);
            List<string> processTypeList = node.QuerySelectorAll("span").Select(el => el.InnerText).ToList();
            await context.Orders.AddAsync(
                new Order
                {
                    OrderNumber = dateAndNumber[0],
                    NameDetail = node.ParentNode.QuerySelector("div.col-md-8>h4>a").InnerText,
                    PublicationDate = dateAndNumber[1].Replace("Открыт", ""),
                    Circulation = orderInfo[0],
                    Price = orderInfo[1],
                    ExpirationDate = orderInfo[2],
                    ProcessingTypes = processTypeList,
                    Description = descriptionDocument.QuerySelector("div.card-text>p:last-child").InnerText
                        .Replace("&mdash;", "—")
                        .Replace("&hellip;", "...")
                        .Replace("&nbsp;", " ")
                });
            await context.SaveChangesAsync();
        }        
    }
}
