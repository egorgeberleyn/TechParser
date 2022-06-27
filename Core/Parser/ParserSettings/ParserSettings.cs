using HtmlAgilityPack;

namespace TechParser.Core.Parser
{
    public class ParserSettings 
    {
        public ParserSettings(string baseUrl)
        {
            BaseUrl = baseUrl; 
            StartPoint = 1;  //начальная стр      
        }

        public string BaseUrl { get; set; }
        public string Prefix { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }

        public HtmlDocument GetHtmlDocument() //генерация html документа
        {
            var web = new HtmlWeb();
            string url = $"{BaseUrl}";
            if(Prefix != null)
                url = $"{BaseUrl}{Prefix}";
            var htmlDocument = web.Load(url);
            return htmlDocument;
        }       
    }
}
