using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Net;
using HtmlAgilityPack;

namespace BackendProject
{
    public static class DataExtractor
    {
        public static string webPageDir = Environment.CurrentDirectory + @"\webpages";
        public static string xmlDir = Environment.CurrentDirectory + @"\xmls";

        public static string DownloadWebpages(int number)
        {
            string url = @"https://www.iatiregistry.org/publisher/undp?page=" + number;           

            if (!Directory.Exists(webPageDir))
            {
                Directory.CreateDirectory(webPageDir);
            }

            
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader stream = new StreamReader(response.GetResponseStream());
                string responseString = stream.ReadToEnd();
                stream.Close();

                string path = webPageDir + @"\" + number + ".html";

                using (StreamWriter streamWrite = new StreamWriter(path, false))
                {
                    streamWrite.WriteLine(responseString);
                }

            return url;
        }

        public static List<string> ParseWebpage(out bool state)
        {
            List<string> xmlLinks = new List<string>();
            if (!Directory.Exists(webPageDir) || Directory.GetFiles(webPageDir).Length != 8) //TO DO!
            {
                state = false;
                return xmlLinks;              
            }
            else
            {
                foreach (var file in Directory.GetFiles(webPageDir))
                {
                    string path = file;
                    HtmlDocument htmlDoc = new HtmlDocument();
                    
                    htmlDoc.Load(path);

                    HtmlNodeCollection itemNodes = htmlDoc.DocumentNode.SelectNodes(@"/html/body//div[@class='dataset-content']/p[3]/a[2]");

                    foreach (var content in itemNodes)
                    {
                        var xmlLink = content.GetAttributeValue("href", "no value");
                        Console.WriteLine(xmlLink);
                        if(xmlLink != "no value")
                        {
                            xmlLinks.Add(xmlLink);
                        }
                    }

                }
                state = true;
                return xmlLinks;
            }
        }
    }
}
