using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            for(int i = 1; i <= 8; i++)
            {
                processListBox.Items.Add(DataExtractor.DownloadWebpages(i));
                filesProcessedLabel.Text = "Files Processed: " + processListBox.Items.Count.ToString();
            }
        }

        private void xmlExtractButton_Click(object sender, EventArgs e)
        {
            if (!DataExtractor.ParseWebpage())
            {
                MessageBox.Show("No data to extract.");
            }
        }
    }
}
