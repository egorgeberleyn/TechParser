using HtmlAgilityPack;

namespace TechParser.Core.Interfaces
{
    public interface IParserSettings
    {
        string BaseUrl { get; set; }

        string Prefix { get; set; }

        int StartPoint { get; set;}
        int EndPoint { get; set; }

        public HtmlDocument GetHtmlDocument();
    }
}
