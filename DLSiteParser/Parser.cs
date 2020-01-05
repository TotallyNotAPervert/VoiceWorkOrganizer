using HtmlAgilityPack;

namespace DLSiteParser
{
    public class Parser
    {
        private HtmlDocument _htmlDocument;

        public Parser(string url)
        {
            var web = new HtmlWeb();
            _htmlDocument = web.Load(url);
        }

        public HtmlNodeCollection GetNodes(string htmlElement, string htmlAttribute, string htmlAttributeValue)
        {
            return GetSingleNode(htmlElement, htmlAttribute, htmlAttributeValue).ChildNodes;
        }

        public HtmlNode GetSingleNode(string htmlElement, string htmlAttribute, string htmlAttributeValue)
        {
            return _htmlDocument.DocumentNode.SelectSingleNode($"//{htmlElement}[@{htmlAttribute}='{htmlAttributeValue}']");
        }
    }
}
