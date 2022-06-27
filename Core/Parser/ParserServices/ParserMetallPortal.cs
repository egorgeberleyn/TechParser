using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack.CssSelectors.NetCore;
using TechParser.Core.Data;
using TechParser.Models;
using TechParser.Storage;

namespace TechParser.Core.Parser
{
    public class MetallPortalParser
    {
        public MetallPortalParser(string url, IStorage storage)
        {
            _parserSettings = new ParserSettings(url);
            _storage = storage;
        }

        private readonly ParserSettings _parserSettings;
        private IStorage _storage;

        public void ParseProvider()
        {
            try
            {
                _parserSettings.Prefix = "zakazi?page=1";
                var htmlDocument = _parserSettings.GetHtmlDocument();
                _parserSettings.EndPoint = 479;  /*int.Parse(htmlDocument.QuerySelectorAll("li.list-inline-item")
                    .Last().InnerText);*/
                int resId = _storage.GetResourceId("MetallPortal");
                var contextProviders = _storage.GetContextProviders(resId);
                
                for (int i = _parserSettings.StartPoint; i <= _parserSettings.EndPoint; i++)
                {
                    _parserSettings.Prefix = $"/katalog?page={i}";
                    var document = _parserSettings.GetHtmlDocument();
                    var companyCardCollection = document.QuerySelectorAll("div.card-body");
                    
                    
                    foreach (var companyCard in companyCardCollection)
                    {
                        var companyData = companyCard.QuerySelectorAll("div.col-md-12.mt-4 > span");
                        var adressData = companyData[0].InnerText;
                        
                        var arry = adressData.Split(',');
                        string adress = string.Empty;
                        if (arry.Length >= 1) for (int j = 1; j < arry.Length; j++) { adress += arry[j];}
                        
                        var companyName = Regex.Replace(companyCard.QuerySelector("h3.h5.d-inline > a").InnerText, "[\n ]+", " ")
                            .Replace("&raquo;", " ")
                            .Replace("&quot;", " ");
                        var shortCompanyDesc = "";
                        if (companyCard.InnerHtml.Contains("class=\"mt-3 order-content\""))
                        {
                            shortCompanyDesc = Regex.Replace(companyCard.QuerySelector("div.mt-3.order-content").InnerText, "[ \t\n]+", " ");
                        }
                        
                        if (contextProviders.Contains(companyName)) continue;
                        _storage.AddProvider(new Provider
                        {
                            CompanyName = companyName,
                            Adress = adress,
                            ResourceId = resId,
                            CompanyDescription = shortCompanyDesc,
                            Phone = companyData[1].InnerText,
                            TypesOfServices = companyCard.QuerySelectorAll("div.mt-2 > span").Select(node => node.GetAttributeValue("data-content", "")).ToList(),
                            City = arry[0],
                        });
                    }
                }
            }
            catch (WebException e)
            {
                throw new WebException("error", e);
            }
        }

        public void ParseOrder()
        {
            
            try
            {
                _parserSettings.Prefix = "zakazi?page=1";
                var htmlDocument = _parserSettings.GetHtmlDocument();
                _parserSettings.EndPoint = 823; /*int.Parse(htmlDocument.QuerySelectorAll("li.list-inline-item")
                    .Last().InnerText);*/
                int resId = _storage.GetResourceId("MetallPortal");
                var contextOrders = _storage.GetContextOrders(resId);
                for (int i = _parserSettings.StartPoint; i <= _parserSettings.EndPoint; i++)
                {
                    _parserSettings.Prefix = $"/zakazi?page={i}";
                    var document = _parserSettings.GetHtmlDocument();
                    var nodeCollection = document.QuerySelectorAll("div.card-body"); // коллекция node
                    foreach (var variableNode in nodeCollection)
                    {
                        
                        var status = variableNode.QuerySelector("h5.card-title > span").InnerText; 
                        var titleAndNum = Regex.Replace(variableNode.QuerySelector("h5.card-title > a").InnerText, "[ \n]+", " ").Split(" ");
                        var orderNum = titleAndNum[1];
                        if (contextOrders.Contains(orderNum))
                            continue;
                        
                        var title = string.Empty; // название 
                        if (titleAndNum.Length >= 1) for (int j = 2; j < titleAndNum.Length; j++) {title += titleAndNum[j] + " ";}

                        var data = Regex.Replace(variableNode.QuerySelector("div.col-md-12.text-muted.my-2").InnerText, "[\n ]+", " ").Split(',');
                        var city = data[1].Replace("Металлообработка", "");
                        
                        var dateRow = variableNode.QuerySelector("div.p-2.my-2.border.border-secondary.rounded" + ">div.row" + ">span:last-child").InnerText.Split(':');
                        var date = dateRow[1];
                        
                        var descriptionList = variableNode.QuerySelectorAll("div.mt-3.order-content > p").Select(node => node.QuerySelector("p").InnerText).ToList();
                        var desc = Regex.Replace(string.Join("", descriptionList.ToArray()), "[ \t\n]+", " ");
                        var circAndPrice = variableNode.QuerySelectorAll("div.p-2.my-2.border.border-secondary.rounded>div.row>span.col-md-4>span");

                        // переход в описание 
                        var titleLink = variableNode.QuerySelector("h5.card-title > a")
                            .GetAttributeValue("href", "").Split("com");
                        _parserSettings.Prefix = titleLink[1];
                        var deskDocument = _parserSettings.GetHtmlDocument();
                        
                        // download archive 
                        var cardBody = deskDocument.QuerySelectorAll("div.card-body")[1];
                        var downloadDocument = string.Empty;
                        if (cardBody.QuerySelector("div.col-md-12").InnerHtml.Contains("class=\"text-dark\""))
                        {
                            downloadDocument = cardBody.QuerySelector("a.text-dark").GetAttributeValue("href", "");
                        }
                        
                        
                        Order order = new  Order()
                        {
                            OrderNumber = orderNum,
                            NameDetail = title,
                            ResourceId = resId,
                            Status = status == "Открыт" ? OrderStatus.Active : OrderStatus.Archive,
                            DownloadFileUrl = downloadDocument,
                            ExpirationDate = date,
                            Adress = city,
                            Description = desc,
                            Price = circAndPrice[3].InnerText,
                            Circulation = circAndPrice[1].InnerText,
                        };
                        _storage.AddOrder(order);
                        
                        if (deskDocument.QuerySelector("div.card.order.my-3").InnerHtml.Contains("class=\"border-top p-4 border-light\""))
                        {
                            var suggestionCollection = deskDocument.QuerySelectorAll("div.border-top.p-4.border-light");
                            foreach (var suggestion in suggestionCollection)
                            {
                                var sugTime = suggestion.QuerySelector("div.bg-light.p-3 > span.text-muted").InnerText;
                                var sugPriceData = suggestion.QuerySelectorAll("div.float-left > div.mt-2")
                                    .Select(el => el.InnerText).ToList();
                                var sugCommentData = suggestion.QuerySelectorAll("div.bg-light.p-3 > p").Select(el => el.InnerText
                                    .Replace("@[\n]", "")).ToList();
                                var sugPrice = sugPriceData[2].Split("Т");
                                var sugContactInfo = suggestion.QuerySelector("div.bg-light.p-3 > span.text-dark.border-bottom.border-dark.mr-3.font-weight-bold").InnerText;
                                
                                _storage.AddSuggestions(new Suggestion()
                                {
                                    OrderId = order.Id,
                                    Time = sugTime,
                                    Price = Regex.Replace(sugPrice[0], "[\n ]+" ," "),
                                    Comment = Regex.Replace(sugCommentData[1], "[\n ]+", " "),
                                    ContactInfo = sugContactInfo,
                                });
                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {   
                throw new WebException("Error", ex);
            }
        }
    }
}
