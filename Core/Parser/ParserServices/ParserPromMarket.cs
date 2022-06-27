using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack.CssSelectors.NetCore;
using TechParser.Core.Data;
using TechParser.Models;
using TechParser.Storage;

namespace TechParser.Core.Parser;

public class ParserPromMarket
{
    private readonly IStorage _storage;
    private ParserSettings _parserSettings;

    public ParserPromMarket(string url, IStorage storage)
    {
        _parserSettings = new ParserSettings(url);
        _storage = storage;
    }

    public void ParseProviders()
    {
        try
        {
            _parserSettings.EndPoint = 10;
            int resId = _storage.GetResourceId("ПромМаркет");
            var contextProviders = _storage.GetContextProviders(resId);
            for (int i = _parserSettings.StartPoint; i <= _parserSettings.EndPoint; i++)
            {
                _parserSettings.Prefix = $"/companies/?PAGEN_1={i}";
                var document = _parserSettings.GetHtmlDocument();
                var companyCardCollection = document.QuerySelectorAll("div.card");
                foreach (var card in companyCardCollection)
                {
                    var companyName = card.QuerySelector("div.card-top>div.uk-h3").InnerText.Replace("&quot;", "");
                    
                    if (contextProviders.Contains(companyName))
                        continue;
                    
                    var cardLink = card.QuerySelector("div.card > a").GetAttributeValue("href", "");
                    
                    _parserSettings.Prefix = cardLink;
                    var cardDoc = _parserSettings.GetHtmlDocument();
                    var cardBody = cardDoc.QuerySelector("div.card-body");
                    
                    var innValue = string.Empty;
                    if (cardBody.InnerHtml.Contains("class=\"card-left\""))
                    {
                        innValue = Regex.Replace(cardDoc.QuerySelector("div.card-left >div:first-child").InnerText, "@[ \n\t]+", "");
                    }
                    
                    var adress = string.Empty;
                    if (cardBody.InnerHtml.Contains("class=\"address\""))
                    {
                        var adressMasSplit = cardDoc.QuerySelector("div.address").InnerText.Split("Адрес");
                        var adressList = adressMasSplit[1].Split("Показать");
                        adress = adressList[0];
                    }
                    var phone = cardDoc
                        .QuerySelector("div.company-contacts" + " > div:first-child " + "> span.information").InnerText;

                    var deskNode = cardDoc.QuerySelector("div.detail-bottom");
                    var desc = string.Empty;
                    if (deskNode.InnerHtml.Contains("class=\"detail-bottom__desc\""))
                        desc = cardDoc.QuerySelector("div.detail-bottom__desc").InnerText.Replace("&nbsp;", "");

                    var servs = cardDoc.QuerySelectorAll("ul#services-link > li > div.uk-accordion-content > ul > li > a.uk-accordion-title")
                        .Select(el => Regex.Replace(el.InnerText, "[\n ]+", " ")).ToList();
                    _storage.AddProvider(new Provider()
                    {
                        CompanyName = Regex.Replace(companyName, "[\n ]+", " "),
                        ResourceId = resId,
                        TaxpayerIdentificationNumber = Regex.Replace(innValue, "[\n ]+", " "),
                        CompanyDescription = Regex.Replace(desc, "[\n ]+", " ").Replace("&quot", " "),
                        Adress = Regex.Replace(adress, "[\n ]+", " ")
                            .Replace("[&amp;]", "")
                            .Replace("[amp;]", ""),
                        Phone = Regex.Replace(phone, "[\n ]+", " "),
                        TypesOfServices = servs,
                    });
                }
            }
        }
        catch (WebException e)
        {
            throw new WebException("Error", e);
        }
    }
}
