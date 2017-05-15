using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Net;
using HtmlAgilityPack;
using System.Xml.Linq;
using System.Windows.Forms;

namespace BackendProject
{
    public static class DataExtractor
    {
        public static string webPageDir = Environment.CurrentDirectory + @"\webpages";
        public static string UNDPDir = Environment.CurrentDirectory + @"\UNDPxmls";
        public static string xmlDir = Environment.CurrentDirectory + @"\otherXmlData";
        

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
                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

                    htmlDoc.Load(path);

                    HtmlNodeCollection itemNodes = htmlDoc.DocumentNode.SelectNodes(@"/html/body//div[@class='dataset-content']/p[3]/a[2]");

                    foreach (var content in itemNodes)
                    {
                        var xmlLink = content.GetAttributeValue("href", "no value");

                        if (xmlLink != "no value")
                        {
                            xmlLinks.Add(xmlLink);
                        }
                    }

                }
                xmlLinks.RemoveAt(xmlLinks.Count - 1);
                foreach (var xml in xmlLinks)
                {
                    Console.WriteLine(xml);
                }
                state = true;
                return xmlLinks;
            }
        }

        public static void ParseXmlData(string xmlLink)
        {
            if (!Directory.Exists(UNDPDir))
            {
                Directory.CreateDirectory(UNDPDir);
            }

            if (xmlLink.Split('/').Last() != "global_projects.xml") {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlLink);
                xmlDoc.Save(UNDPDir + @"\" + xmlLink.Split('/').Last());
            }
        }

        public static void CoutriesCsvToXml(string csvPath)
        {
            if (!Directory.Exists(xmlDir))
            {
                Directory.CreateDirectory(xmlDir);
            }

            try
            {
                string[] csvDocument = File.ReadAllLines(csvPath);
                XElement root = new XElement("countries",
                from line in csvDocument
                let fields = line.Split(';')
                select new XElement("country",
                    new XElement("name", fields[0].Trim()),
                    new XElement("code", fields[3].Trim()),
                    new XElement("population", fields[6].Trim())
                    )
                );
                root.FirstNode.Remove();
                root.Save(xmlDir + @"\countriesPopulation.xml");
            }

            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void CountriesCodesCsvToXml(string csvPath)
        {
            if (!Directory.Exists(xmlDir))
            {
                Directory.CreateDirectory(xmlDir);
            }

            try
            {
                string[] csvDocument = File.ReadAllLines(csvPath);
                XElement root = new XElement("countries",
                from line in csvDocument
                let fields = line.Split(',')
                select new XElement("country",
                    new XElement("name", fields[0].Trim()),
                    new XElement("code", fields[1].Trim()),
                    new XElement("number_code", fields[3].Trim())
                    )
                );
                root.FirstNode.Remove();
                root.Save(xmlDir + @"\countriesCodes.xml");
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void XmlConnector()
        {
            XmlDocument populationDoc = new XmlDocument();
            populationDoc.Load(xmlDir + @"\countriesPopulation.xml");
            XmlDocument codesDoc = new XmlDocument();
            codesDoc.Load(xmlDir + @"\countriesCodes.xml");

            XmlNodeList populationNodes = populationDoc.GetElementsByTagName("country");
            XmlNodeList codesNodes = codesDoc.GetElementsByTagName("country");            

            foreach (XmlNode node in codesNodes)
            {
                string alphaCode = node.SelectSingleNode("./code").InnerText;

                foreach (XmlNode popNode in populationNodes)
                {
                    string alphaCode2 = popNode.SelectSingleNode("./code").InnerText;

                    if (alphaCode == alphaCode2)
                    {
                        string population = popNode.SelectSingleNode("./population").InnerText.Trim();
                        XmlNode populationNode = codesDoc.CreateElement("population");
                        populationNode.InnerText = population;
                        node.AppendChild(populationNode);
                        break;
                    }
                }

                codesDoc.Save(xmlDir + @"\countriesCodesPopulation.xml");
            }
        }

        /*public static XmlNode ExtractUNDP(string filePath)
        {
            XmlDocument UNDPxml = new XmlDocument();
            XmlDocument codesXml = new XmlDocument();
            XmlDocument worldBankXml = new XmlDocument();
            UNDPxml.Load(filePath);
            codesXml.Load("codes_with_population.xml");
            worldBankXml.Load("IATI_ORG.xml");
                        
        }*/


    }
}
