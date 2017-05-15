namespace BackendProject
{
    partial class backendMainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.webPageDownloadButton = new System.Windows.Forms.Button();
            this.processListBox = new System.Windows.Forms.ListBox();
            this.filesProcessedLabel = new System.Windows.Forms.Label();
            this.xmlDownloadButton = new System.Windows.Forms.Button();
            this.openVisButton = new System.Windows.Forms.Button();
            this.webpageDownloader = new System.ComponentModel.BackgroundWorker();
            this.xmlDownloader = new System.ComponentModel.BackgroundWorker();
            this.extractXmlButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // webPageDownloadButton
            // 
            this.webPageDownloadButton.Location = new System.Drawing.Point(290, 30);
            this.webPageDownloadButton.Name = "webPageDownloadButton";
            this.webPageDownloadButton.Size = new System.Drawing.Size(75, 50);
            this.webPageDownloadButton.TabIndex = 0;
            this.webPageDownloadButton.Text = "Download/Update Webpages";
            this.webPageDownloadButton.UseVisualStyleBackColor = true;
            this.webPageDownloadButton.Click += new System.EventHandler(this.webPageDownloadButton_Click);
            // 
            // processListBox
            // 
            this.processListBox.FormattingEnabled = true;
            this.processListBox.Location = new System.Drawing.Point(15, 50);
            this.processListBox.Name = "processListBox";
            this.processListBox.Size = new System.Drawing.Size(255, 264);
            this.processListBox.TabIndex = 1;
            // 
            // filesProcessedLabel
            // 
            this.filesProcessedLabel.AutoSize = true;
            this.filesProcessedLabel.Location = new System.Drawing.Point(15, 30);
            this.filesProcessedLabel.Name = "filesProcessedLabel";
            this.filesProcessedLabel.Size = new System.Drawing.Size(93, 13);
            this.filesProcessedLabel.TabIndex = 2;
            this.filesProcessedLabel.Text = "Files Processed: 0";
            // 
            // xmlDownloadButton
            // 
            this.xmlDownloadButton.Location = new System.Drawing.Point(290, 110);
            this.xmlDownloadButton.Name = "xmlDownloadButton";
            this.xmlDownloadButton.Size = new System.Drawing.Size(75, 50);
            this.xmlDownloadButton.TabIndex = 0;
            this.xmlDownloadButton.Text = "Download XML Data";
            this.xmlDownloadButton.UseVisualStyleBackColor = true;
            this.xmlDownloadButton.Click += new System.EventHandler(this.xmlExtractButton_Click);
            // 
            // openVisButton
            // 
            this.openVisButton.Location = new System.Drawing.Point(290, 260);
            this.openVisButton.Name = "openVisButton";
            this.openVisButton.Size = new System.Drawing.Size(75, 50);
            this.openVisButton.TabIndex = 0;
            this.openVisButton.Text = "Open Visualisation";
            this.openVisButton.UseVisualStyleBackColor = true;
            // 
            // webpageDownloader
            // 
            this.webpageDownloader.WorkerReportsProgress = true;
            this.webpageDownloader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.webpageDownloader_DoWork);
            this.webpageDownloader.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.webpageDownloader_ProgressChanged);
            // 
            // xmlDownloader
            // 
            this.xmlDownloader.WorkerReportsProgress = true;
            this.xmlDownloader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.xmlExtractor_DoWork);
            this.xmlDownloader.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.xmlExtractor_ProgressChanged);
            // 
            // extractXmlButton
            // 
            this.extractXmlButton.Location = new System.Drawing.Point(290, 185);
            this.extractXmlButton.Name = "extractXmlButton";
            this.extractXmlButton.Size = new System.Drawing.Size(75, 50);
            this.extractXmlButton.TabIndex = 0;
            this.extractXmlButton.Text = "Extract XML Data";
            this.extractXmlButton.UseVisualStyleBackColor = true;
            this.extractXmlButton.Click += new System.EventHandler(this.xmlExtractButton_Click);
            // 
            // backendMainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 330);
            this.Controls.Add(this.filesProcessedLabel);
            this.Controls.Add(this.processListBox);
            this.Controls.Add(this.openVisButton);
            this.Controls.Add(this.extractXmlButton);
            this.Controls.Add(this.xmlDownloadButton);
            this.Controls.Add(this.webPageDownloadButton);
            this.Name = "backendMainWindow";
            this.Text = "Xml Analyzer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button webPageDownloadButton;
        private System.Windows.Forms.ListBox processListBox;
        private System.Windows.Forms.Label filesProcessedLabel;
        private System.Windows.Forms.Button xmlDownloadButton;
        private System.Windows.Forms.Button openVisButton;
        private System.ComponentModel.BackgroundWorker webpageDownloader;
        private System.ComponentModel.BackgroundWorker xmlDownloader;
        private System.Windows.Forms.Button extractXmlButton;
    }
}

