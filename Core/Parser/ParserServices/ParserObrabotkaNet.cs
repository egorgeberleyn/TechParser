using HtmlAgilityPack.CssSelectors.NetCore;
using TechParser.Core.Data;
using TechParser.Models;
using TechParser.Storage;

namespace TechParser.Core.Parser
{
    public class ParserObrabotkaNet
    {
        public ParserObrabotkaNet(string url, IStorage storage)
        {
            parserSettings = new ParserSettings(url); //добавление настроек парсера
            _storage = storage;

        }
        private readonly ParserSettings parserSettings;
        private readonly IStorage _storage;
        
        public void ParseActiveOrders()
        {
            parserSettings.Prefix = @"/orders/?page=1";
            var htmlDocument = parserSettings.GetHtmlDocument();

            var resourceID = _storage.GetResourceId("obrabotka.net");
            var orderNumbers = _storage.GetActiveOrders(resourceID);

            parserSettings.EndPoint =  
                int.Parse(htmlDocument.QuerySelectorAll("ul.pagination>li>a")
                .Skip(3)
                .First()
                .InnerText);

            for (int i = parserSettings.StartPoint; i < parserSettings.EndPoint; i++)
            {
                parserSettings.Prefix = $"/orders/?page={i}";
                var document = parserSettings.GetHtmlDocument();
                var nodeCollection = document.QuerySelectorAll("div.order-item"); //карточки заказов
               
                foreach (var node in nodeCollection)
                {
                    var dateAndNumber = node.QuerySelector("div.panel-heading>a:last-child").InnerText.Split("от", StringSplitOptions.RemoveEmptyEntries);
                    var currentOrderNumber = dateAndNumber[0].Trim();
                    if (orderNumbers.Contains(currentOrderNumber))
                        continue;

                    parserSettings.Prefix = node.QuerySelector("div.panel-heading>a:last-child").Attributes[0].Value;
                    var descriptionDocument = parserSettings.GetHtmlDocument();

                    List<string> processTypeList = node.QuerySelectorAll("a.label")
                        .Select(el => el.InnerText.Trim()).ToList();

                    var desc = node.QuerySelector("div.panel-body").InnerText
                        .Trim()
                        .Replace("&nbsp;", " ");

                    var downloadFileUrl = string.Empty;
                    if (descriptionDocument.QuerySelector("body").InnerHtml.Contains("class=\"btn btn-default list-group-item\""))
                    {
                        downloadFileUrl = $"{parserSettings.BaseUrl}{descriptionDocument.QuerySelector("a.btn.btn-default.list-group-item").Attributes[1].Value}";
                    }
                    else if(descriptionDocument.QuerySelector("body").InnerHtml.Contains("class=\"order-file list-group-item\""))
                    {
                        downloadFileUrl = $"{parserSettings.BaseUrl}{descriptionDocument.QuerySelector("a.order-file").Attributes[1].Value}";
                    }

                    var order = new Order
                    {
                        OrderNumber = currentOrderNumber,
                        PublicationDate = dateAndNumber[1].Trim(),
                        ProcessingTypes = processTypeList,
                        Description = desc,
                        ResourceId = resourceID,
                        Status = OrderStatus.Active,
                        DownloadFileUrl = downloadFileUrl
                    };
                    _storage.AddOrder(order);
                    

                    if (descriptionDocument.QuerySelector("body").InnerHtml.Contains("h3"))
                    {
                        var suggestionCollection = descriptionDocument.QuerySelectorAll("div.panel");

                        foreach (var suggestion in suggestionCollection)
                        {
                            var contactInfo = suggestion.QuerySelectorAll("a").Select(el => el.InnerText.Replace("&quot;", "\"")).ToList();
                            var conditions = suggestion.QuerySelector("div.col-sm-6").InnerText.Trim().Split('\n');

                            var comment = suggestion.QuerySelector("div.panel-body").InnerText.Replace("&nbsp;", " ").Trim().Split('\n')[0];

                            _storage.AddSuggestions(new Suggestion
                            {
                                OrderId = order.Id,
                                ContactInfo = string.Join(" ", contactInfo),
                                Comment = comment,
                                Time = conditions[0],
                                Price = conditions[1].Trim()                                
                            });
                        }                                                
                    }                    
                }
            }            
        }

        public void ParseArchiveOrders()
        {
            parserSettings.Prefix = @"/orders/archive/?page=1";
            var htmlDocument = parserSettings.GetHtmlDocument();

            var resourceID = _storage.GetResourceId("obrabotka.net");
            var orderNumbers = _storage.GetArchiveOrders(resourceID);

            parserSettings.EndPoint =  int.Parse(htmlDocument.QuerySelectorAll("li.hidden-xs")
                .Last()
                .InnerText); 
            
            for (int i = parserSettings.StartPoint; i < parserSettings.EndPoint; i++)
            {
                parserSettings.Prefix = $"/orders/archive/?page={i}";
                var document = parserSettings.GetHtmlDocument();
                var nodeCollection = document.QuerySelectorAll("div.order-item"); //карточки заказов
                
                foreach (var node in nodeCollection)
                {
                    var dateAndNumber = node.QuerySelector("div.panel-heading>a:last-child").InnerText.Split("от", StringSplitOptions.RemoveEmptyEntries);
                    var currentOrderNumber = dateAndNumber[0].Trim();
                    if (orderNumbers.Contains(currentOrderNumber))
                        continue;

                    parserSettings.Prefix = node.QuerySelector("div.panel-heading>a:last-child").Attributes[0].Value;
                    var descriptionDocument = parserSettings.GetHtmlDocument();

                    List<string> processTypeList = node.QuerySelectorAll("a.label")
                        .Select(el => el.InnerText.Trim()).ToList();

                    var desc = node.QuerySelector("div.panel-body").InnerText
                        .Trim()
                        .Replace("&nbsp;", " ");

                    var downloadFileUrl = string.Empty;
                    if (descriptionDocument.QuerySelector("body").InnerHtml.Contains("class=\"btn btn-default list-group-item\""))
                    {
                        downloadFileUrl = $"{parserSettings.BaseUrl}{descriptionDocument.QuerySelector("a.btn.btn-default.list-group-item").Attributes[1].Value}";
                    }
                    else if (descriptionDocument.QuerySelector("body").InnerHtml.Contains("class=\"order-file list-group-item\""))
                    {
                        downloadFileUrl = $"{parserSettings.BaseUrl}{descriptionDocument.QuerySelector("a.order-file").Attributes[1].Value}";
                    }

                    var order = new Order
                    {
                        OrderNumber = currentOrderNumber,
                        PublicationDate = dateAndNumber[1].Trim(),
                        ProcessingTypes = processTypeList,
                        Description = desc,
                        ResourceId = resourceID,
                        Status = OrderStatus.Archive,
                        DownloadFileUrl = downloadFileUrl
                    };
                    _storage.AddOrder(order);

                    if (descriptionDocument.QuerySelector("body").InnerHtml.Contains("h3"))
                    {
                        var suggestionCollection = descriptionDocument.QuerySelectorAll("div.panel");

                        foreach (var suggestion in suggestionCollection)
                        {
                            var contactInfo = suggestion.QuerySelectorAll("a").Select(el => el.InnerText.Replace("&quot;", "\"")).ToList();
                            var conditions = suggestion.QuerySelector("div.col-sm-6").InnerText.Trim().Split('\n');
                            if (conditions.Length != 2)
                                continue;
                            
                            var comment = suggestion.QuerySelector("div.panel-body").InnerText.Replace("&nbsp;", " ").Trim().Split('\n')[0];
                            
                            _storage.AddSuggestions(new Suggestion
                            {
                                OrderId = order.Id,
                                ContactInfo = string.Join(" ", contactInfo),
                                Comment = comment,
                                Time = conditions[0],
                                Price = conditions[1].Trim()
                            });
                        }                        
                    }                    
                }
            }            
        }


        public void ParseProviders()
        {
            parserSettings.Prefix = @"/companies/?page=1";
            var htmlDocument = parserSettings.GetHtmlDocument();

            var resourceID = _storage.GetResourceId("obrabotka.net");
            var providerNames = _storage.GetContextProviders(resourceID);

            parserSettings.EndPoint = int.Parse(htmlDocument.QuerySelectorAll("ul.pagination>li>a")
                .Skip(3)
                .First()
                .InnerText);
            
            for (int i = parserSettings.StartPoint; i < parserSettings.EndPoint; i++)
            {
                parserSettings.Prefix = $"/companies/?page={i}";
                var document = parserSettings.GetHtmlDocument();
                var nodeCollection = document.QuerySelectorAll("div.company-item"); //карточки заказов

                foreach (var node in nodeCollection)
                {
                    var providerName = node.QuerySelector("b>a").InnerText.Trim().Replace("&quot;", "\"");
                    if (providerNames.Contains(providerName))
                        continue;

                    var providerDescription = "";
                    if (node.InnerHtml.Contains("<div class=\"col-md-8\">"))
                        providerDescription = node.QuerySelector("div.col-md-8").InnerText.Trim();
                    else
                    {
                        parserSettings.Prefix = node.QuerySelector("a").Attributes[0].Value;
                        var descriptionDocument = parserSettings.GetHtmlDocument();
                        providerDescription = descriptionDocument.QuerySelector("p").InnerText.Trim();
                    }

                    _storage.AddProvider(new Provider
                    {
                        ResourceId = resourceID,
                        CompanyName = providerName,
                        Adress = node.QuerySelector("span").InnerText.Trim(),
                        CompanyDescription = providerDescription.Replace("&quot;", "\"").Trim(),
                        Email = node.QuerySelector("ul").InnerText.Trim(),
                        Phone = node.QuerySelector("div.col-md-4>ul:last-child").InnerText.Trim()
                    });                    
                }
            }
        }

    }
}
