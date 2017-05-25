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
                char[] num_id_chars = { ' ', '\"' };
                XElement root = new XElement("countries",
                from line in csvDocument
                let fields = line.Split(',')
                select new XElement("country",
                    new XElement("name", fields[0].Trim()),
                    new XElement("code", fields[1].Trim()),
                    new XAttribute("num_id", fields[3].Trim(num_id_chars))
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

        public static XmlNode ExtractWorldBank()
        {
            XmlDocument outputXml = new XmlDocument();
            //XmlDocument UNDPxml = new XmlDocument();
            XmlDocument codesXml = new XmlDocument();
            XmlDocument worldBankXml = new XmlDocument();
            //UNDPxml.Load(filePath);
            codesXml.Load("otherXmlData/countriesCodesPopulation.xml");
            worldBankXml.Load("otherXmlData/worldBank.xml");

            /*
            XmlElement root;
            root = outputXml.CreateElement("countries");
            outputXml.AppendChild(root);
            */

            var countryInfos = new List<CountryData>();
            string lastUsedCode = null;

            foreach (XmlNode bankNode in worldBankXml.FirstChild.FirstChild.ChildNodes)
            {
                if (bankNode.Name == "recipient-country-budget")
                {
                    var countryCode = bankNode.FirstChild.Attributes[0].Value;
                    var year = bankNode.LastChild.Attributes[1].Value;
                    var budget = bankNode.LastChild.InnerText;
                    System.Diagnostics.Debug.WriteLine(" X " + countryCode + " X " + year + " X " + budget);

                    string processedCode = null;

                    foreach (var info in countryInfos)
                    {
                        if (info.CountryCode == countryCode)
                        {

                        }
                    }

                    /*
                    foreach (XmlNode codeNode in codesXml.FirstChild.NextSibling.ChildNodes)
                    {
                        var numCode = codeNode.FirstChild.NextSibling.InnerText;
                        numCode.Trim('\"');

                        System.Diagnostics.Debug.WriteLine("KKK " + numCode.Trim('\"'));

                        if (countryCode.Equals(numCode.Trim('\"')))
                        {
                            System.Diagnostics.Debug.WriteLine("IF funguje");
                        }
                    }
                    */

                    lastUsedCode = countryCode;
                }
            }


            //outputXml.Save(xmlDir + @"\finalOutput.xml");

            return outputXml;
        }

        public static void FinalXmlToJson(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            StreamWriter sw = new StreamWriter(xmlDir, false);
            sw.WriteLine("[");

            XmlNodeList countries = doc.GetElementsByTagName("country");
            for (int i = 0; i < countries.Count; i++)
            {
                XmlElement node = (XmlElement)countries[i];
                string id = node.GetAttribute("num_id");
                sw.WriteLine("{\"" + id + "\":{");

                string name = node.GetElementsByTagName("name")[0].InnerText;
                sw.WriteLine("\"name\": \"" + name + "\",");

                sw.WriteLine("\"num_id\": \"" + id + "\",");

                string code = node.GetElementsByTagName("code")[0].InnerText;
                sw.WriteLine("\"code\": \"" + code + "\",");

                string population = node.GetElementsByTagName("population")[0].InnerText;
                sw.WriteLine("\"population\": \"" + population + "\",");

                sw.WriteLine("\"data\":{");
                XmlNodeList years = node.GetElementsByTagName("period");

                for (int j = 0; j < years.Count; j++)
                {
                    XmlElement year = (XmlElement)years[j];

                    string currentYear = year.GetAttribute("year");
                    sw.WriteLine("\"" + currentYear + "\":{");
                    XmlNodeList organizations = year.GetElementsByTagName("organization");

                    for (int k = 0; k < organizations.Count; k++)
                    {
                        XmlElement organization = (XmlElement)organizations[k];

                        string org = organization.GetAttribute("name");
                        sw.WriteLine("\"" + org + "\":{");

                        string budget = organization.GetElementsByTagName("budget")[0].InnerText;
                        sw.WriteLine("\"budget\": \"" + budget + "\",");

                        string budgetPopulation = organization.GetElementsByTagName("budget_population")[0].InnerText;
                        sw.WriteLine("\"budget_population\": \"" + budgetPopulation + "\"");

                        if (k == organizations.Count - 1)
                        {
                            sw.WriteLine("}");
                        }
                        else
                        {
                            sw.WriteLine("},");
                        }
                    }

                    sw.WriteLine("},");

                }

                XmlElement sums = (XmlElement)node.GetElementsByTagName("sum")[0];
                XmlNodeList orgsums = sums.GetElementsByTagName("organization");
                sw.WriteLine("\"sum\":{");

                for (int k = 0; k < orgsums.Count; k++)
                {
                    XmlElement sumNode = (XmlElement)orgsums[k];

                    string currentOrg = sumNode.GetAttribute("name");
                    sw.WriteLine("\"" + currentOrg + "\":{");

                    string budget = sumNode.GetElementsByTagName("budget")[0].InnerText;
                    sw.WriteLine("\"budget\": \"" + budget + "\",");

                    string budgetPopulation = sumNode.GetElementsByTagName("budget_population")[0].InnerText;
                    sw.WriteLine("\"budget_population\": \"" + budgetPopulation + "\"");

                    if (k == orgsums.Count - 1)
                    {
                        sw.WriteLine("}");
                    }
                    else
                    {
                        sw.WriteLine("},");
                    }

                }

                sw.WriteLine("}");
                sw.WriteLine("}");

                sw.WriteLine("}");
                if (i == countries.Count - 1)
                {
                    sw.WriteLine("}");
                }
                else
                {
                    sw.WriteLine("},");
                }
            }
            sw.WriteLine("]");
            sw.Close();
        }
    }
    }
}
