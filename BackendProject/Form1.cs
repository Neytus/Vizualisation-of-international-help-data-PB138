using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

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
            filesProcessedLabel.Text = "Files Processed: 0";
            webpageDownloader.RunWorkerAsync();
        }

        private void xmlExtractButton_Click(object sender, EventArgs e)
        {
            
        }

        private void webpageDownloader_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 1; i <= 8; i++)
            {
                webpageDownloader.ReportProgress(i, DataExtractor.DownloadWebpages(i));                
            }
        }

        private void webpageDownloader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            processListBox.Items.Add(e.UserState.ToString());
            filesProcessedLabel.Text = "Files Processed: " + e.ProgressPercentage.ToString();
        }

        
    }
}
