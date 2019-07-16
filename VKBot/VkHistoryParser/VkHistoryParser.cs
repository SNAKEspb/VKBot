using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VkHistoryParser
{
    public class VkHistoryParser
    {
        public static void parse(string directoryPath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encrus = Encoding.GetEncoding(1251);

            var files = Directory.GetFiles(directoryPath);

            var regex = new Regex(@"(\&\#\d*;)|(http.*)|Посмотрите,.*");//to remove smiles, links and other stuff
            
            var messages = files.Select(t =>
                {
                    HtmlDocument htmlSnippet = new HtmlDocument();
                    htmlSnippet.Load(t, encrus);
                    return htmlSnippet;
                })
                .SelectMany(htmlSnippet => 
                    htmlSnippet.DocumentNode.Descendants()
                    .Where(t => t.Name == "a" && t.Attributes.Any(attribute => attribute.Value.Contains("id212515973")) && t.ParentNode.Attributes.Any(attr => attr.Value == "message__header"))
                    .Select(t => t.ParentNode.ParentNode.ParentNode.FirstChild.ChildNodes.First(node => !node.HasAttributes && node.Name == "div").FirstChild)
                    .Where(t => t.Name == "#text")
                    .Select(t => t.InnerText)
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(t => t.Trim())
                    .Select(t => t.Replace("\n", ""))
                    .Select(t => regex.Replace(t, " "))
                    .Where(t => !t.StartsWith("http") || !t.StartsWith("Посмотрите,"))// do not work D:
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                );
            File.AppendAllLines("VityaMessages.txt", messages, encrus);
        }

        private static void parseTest(string directoryPath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encrus = Encoding.GetEncoding(1251);
            //htmlSnippet.LoadHtml(Html);
            var files = Directory.GetFiles(directoryPath);

            HtmlDocument htmlSnippetTest = new HtmlDocument();
            htmlSnippetTest.Load(files[5], encrus);
            var text = htmlSnippetTest.DocumentNode.Descendants().Where(t => t.Name == "a" && t.Attributes.Any(attribute => attribute.Value.Contains("id212515973")));
            var test1 = text.Select(t => t.ParentNode.ParentNode.ParentNode.FirstChild.ChildNodes.First(node => !node.HasAttributes && node.Name == "div").FirstChild);
            var test2 = test1.Where(t => t.Name == "#text");
            var test3 = test2.Select(t => t.InnerText)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .Where(t => !t.StartsWith("http"))
                .Select(t => t.Replace("\n", ""));

            //var dec = System.Net.WebUtility.HtmlDecode("&#129304;");
        }
    }
}
