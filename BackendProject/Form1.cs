using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

namespace BackendProject
{
    public partial class backendMainWindow : Form
    {

        public backendMainWindow()
        {
            InitializeComponent();
        }

        private void webPageDownloadButton_Click(object sender, EventArgs e)
        {
            processListBox.Items.Clear();
            processListBox.Items.Add("Downloading...");
            filesProcessedLabel.Text = "Files Processed: 0";
            webpageDownloader.RunWorkerAsync();
        }

        private void xmlDownloadButton_Click(object sender, EventArgs e)
        {
            processListBox.Items.Clear();
            processListBox.Items.Add("Downloading...");
            filesProcessedLabel.Text = "Files Processed: 0";
            xmlDownloader.RunWorkerAsync();
        }

        private void webpageDownloader_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 1; i <= 8; i++)
            {
                webpageDownloader.ReportProgress(i, DataExtractor.DownloadWebpages(i));                
            }
            webpageDownloader.ReportProgress(-1);
            
        }

        private void webpageDownloader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != -1)
            {
                processListBox.Items.Add(e.UserState.ToString());
                filesProcessedLabel.Text = "Files Processed: " + e.ProgressPercentage.ToString();
            }
            else 
            {
                processListBox.Items.Clear();
                processListBox.Items.Add("All weppage files were successfully downloaded.");
                filesProcessedLabel.Text = "Files Processed: All.";
            }
        }

        private void xmlDownloader_DoWork(object sender, DoWorkEventArgs e)
        {
            bool state;
            
            DataExtractor.CoutriesCsvToXml(@"..\..\\csvFolder\countries_population.csv");
            DataExtractor.CountriesCodesCsvToXml(@"..\..\\csvFolder\countries_codes_and_coordinates.csv");
            DataExtractor.XmlConnector();
            var xmlLinks = DataExtractor.ParseWebpage(out state);
            if (!state)
            {
                MessageBox.Show("No webpages to parse.");
            }
            else
            {
                foreach(var xml in xmlLinks)
                {
                    try
                    {
                        DataExtractor.ParseXmlData(xml);
                        xmlDownloader.ReportProgress(1, xml);
                    }
                    catch(WebException ex)
                    {
                        xmlDownloader.ReportProgress(1, xml.Split('/').Last() + "does not exist");
                    }
                }
            }

            XmlDocument worldBankOrgFile = new XmlDocument();
            worldBankOrgFile.Load("http://siteresources.worldbank.org/INTSOPE/Resources/5929468-1305310586289/IATI_ORG.xml");
            worldBankOrgFile.Save(@".\otherxmlData\worldBank.xml");

            xmlDownloader.ReportProgress(2);
        }

        private void xmlDownloader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 1)
            {
                processListBox.Items.Add(e.UserState);
                filesProcessedLabel.Text = "Files Processed: " + processListBox.Items.Count.ToString();
            }
            else
            {
                processListBox.Items.Clear();
                processListBox.Items.Add("All XML files were successfully downloaded.");
                filesProcessedLabel.Text = "Files Processed: All.";
            }
        }

        private void finalizerButton_Click(object sender, EventArgs e)
        {
            processListBox.Items.Clear();
            filesProcessedLabel.Text = "Files Processed: 0";
            finalizeWorker.RunWorkerAsync();
        }

        private void finalizeWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string xmlDir = Environment.CurrentDirectory + @"\otherXmlData";

            DataExtractor.ExtractWorldBank();
            DataExtractor.ExtractUNDP();

            finalizeWorker.ReportProgress(1, "All the data has been extracted successfully.");
        }


        private void finalizeWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            processListBox.Items.Add(e.UserState);
            filesProcessedLabel.Text = "Files Processed: All";
        }

        private void openVisButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"..\..\..\\Frontend/index.html" );
        }
    }
}
