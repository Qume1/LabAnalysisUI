namespace LabAnalysisUI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabRSD;
        private System.Windows.Forms.TabPage tabDetectionLimit;
        private System.Windows.Forms.TabPage tabVirtualSamples;
        
        // New components
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Label lblFileSelect;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.NumericUpDown numMinStdDev;
        private System.Windows.Forms.NumericUpDown numStartSeconds;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.Label lblMinStdDev;
        private System.Windows.Forms.Label lblStartSeconds;
        private System.Windows.Forms.Button btnSaveOutput;
        private System.Windows.Forms.Button btnShowExceeded;
        private System.Windows.Forms.NumericUpDown numDriftStart;
        private System.Windows.Forms.NumericUpDown numDriftEnd;
        private System.Windows.Forms.Label lblDriftStart;
        private System.Windows.Forms.Label lblDriftEnd;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabRSD = new TabPage();
            lblFileSelect = new Label();
            txtFilePath = new TextBox();
            btnSelectFile = new Button();
            lblMinStdDev = new Label();
            numMinStdDev = new NumericUpDown();
            lblStartSeconds = new Label();
            numStartSeconds = new NumericUpDown();
            btnAnalyze = new Button();
            btnSaveOutput = new Button();
            btnShowExceeded = new Button();
            txtResults = new TextBox();
            lblDriftStart = new Label();
            numDriftStart = new NumericUpDown();
            lblDriftEnd = new Label();
            numDriftEnd = new NumericUpDown();
            tabDetectionLimit = new TabPage();
            tabVirtualSamples = new TabPage();
            openFileDialog1 = new OpenFileDialog();
            tabControl1.SuspendLayout();
            tabRSD.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numMinStdDev).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numStartSeconds).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDriftStart).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDriftEnd).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabRSD);
            tabControl1.Controls.Add(tabDetectionLimit);
            tabControl1.Controls.Add(tabVirtualSamples);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Margin = new Padding(3, 4, 3, 4);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1170, 1024);
            tabControl1.TabIndex = 0;
            // 
            // tabRSD
            // 
            tabRSD.Controls.Add(lblFileSelect);
            tabRSD.Controls.Add(txtFilePath);
            tabRSD.Controls.Add(btnSelectFile);
            tabRSD.Controls.Add(lblMinStdDev);
            tabRSD.Controls.Add(numMinStdDev);
            tabRSD.Controls.Add(lblStartSeconds);
            tabRSD.Controls.Add(numStartSeconds);
            tabRSD.Controls.Add(btnAnalyze);
            tabRSD.Controls.Add(btnSaveOutput);
            tabRSD.Controls.Add(btnShowExceeded);
            tabRSD.Controls.Add(txtResults);
            tabRSD.Controls.Add(lblDriftStart);
            tabRSD.Controls.Add(numDriftStart);
            tabRSD.Controls.Add(lblDriftEnd);
            tabRSD.Controls.Add(numDriftEnd);
            tabRSD.Location = new Point(4, 29);
            tabRSD.Margin = new Padding(3, 4, 3, 4);
            tabRSD.Name = "tabRSD";
            tabRSD.Size = new Size(1162, 991);
            tabRSD.TabIndex = 0;
            tabRSD.Text = "Расчет СКО и дрейфа";
            tabRSD.UseVisualStyleBackColor = true;
            // 
            // lblFileSelect
            // 
            lblFileSelect.AutoSize = true;
            lblFileSelect.Location = new Point(11, 27);
            lblFileSelect.Name = "lblFileSelect";
            lblFileSelect.Size = new Size(195, 20);
            lblFileSelect.TabIndex = 0;
            lblFileSelect.Text = "Выберите текстовый файл:";
            // 
            // txtFilePath
            // 
            txtFilePath.Location = new Point(11, 60);
            txtFilePath.Margin = new Padding(3, 4, 3, 4);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.ReadOnly = true;
            txtFilePath.Size = new Size(457, 27);
            txtFilePath.TabIndex = 1;
            // 
            // btnSelectFile
            // 
            btnSelectFile.Location = new Point(474, 52);
            btnSelectFile.Margin = new Padding(3, 4, 3, 4);
            btnSelectFile.Name = "btnSelectFile";
            btnSelectFile.Size = new Size(114, 42);
            btnSelectFile.TabIndex = 2;
            btnSelectFile.Text = "Обзор...";
            btnSelectFile.UseVisualStyleBackColor = true;
            btnSelectFile.Click += btnSelectFile_Click;
            // 
            // lblMinStdDev
            // 
            lblMinStdDev.AutoSize = true;
            lblMinStdDev.Location = new Point(8, 122);
            lblMinStdDev.Name = "lblMinStdDev";
            lblMinStdDev.Size = new Size(216, 20);
            lblMinStdDev.TabIndex = 3;
            lblMinStdDev.Text = "Минимальное значение СКО:";
            // 
            // numMinStdDev
            // 
            numMinStdDev.DecimalPlaces = 2;
            numMinStdDev.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numMinStdDev.Location = new Point(230, 120);
            numMinStdDev.Margin = new Padding(3, 4, 3, 4);
            numMinStdDev.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numMinStdDev.Name = "numMinStdDev";
            numMinStdDev.Size = new Size(137, 27);
            numMinStdDev.TabIndex = 4;
            numMinStdDev.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            // 
            // lblStartSeconds
            // 
            lblStartSeconds.AutoSize = true;
            lblStartSeconds.Location = new Point(8, 174);
            lblStartSeconds.Name = "lblStartSeconds";
            lblStartSeconds.Size = new Size(174, 20);
            lblStartSeconds.TabIndex = 5;
            lblStartSeconds.Text = "Начальная строка (сек):";
            // 
            // numStartSeconds
            // 
            numStartSeconds.Location = new Point(230, 172);
            numStartSeconds.Margin = new Padding(3, 4, 3, 4);
            numStartSeconds.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numStartSeconds.Name = "numStartSeconds";
            numStartSeconds.Size = new Size(137, 27);
            numStartSeconds.TabIndex = 6;
            numStartSeconds.Value = new decimal(new int[] { 1800, 0, 0, 0 });
            // 
            // btnAnalyze
            // 
            btnAnalyze.Enabled = false;
            btnAnalyze.Location = new Point(872, 116);
            btnAnalyze.Margin = new Padding(3, 4, 3, 4);
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.Size = new Size(160, 33);
            btnAnalyze.TabIndex = 7;
            btnAnalyze.Text = "Анализировать";
            btnAnalyze.Click += btnAnalyze_Click;
            // 
            // btnSaveOutput
            // 
            btnSaveOutput.Enabled = false;
            btnSaveOutput.Location = new Point(12, 781);
            btnSaveOutput.Margin = new Padding(3, 4, 3, 4);
            btnSaveOutput.Name = "btnSaveOutput";
            btnSaveOutput.Size = new Size(137, 33);
            btnSaveOutput.TabIndex = 8;
            btnSaveOutput.Text = "Сохранить отчёт";
            btnSaveOutput.Click += btnSaveOutput_Click;
            // 
            // btnShowExceeded
            // 
            btnShowExceeded.Enabled = false;
            btnShowExceeded.Location = new Point(872, 174);
            btnShowExceeded.Margin = new Padding(3, 4, 3, 4);
            btnShowExceeded.Name = "btnShowExceeded";
            btnShowExceeded.Size = new Size(160, 33);
            btnShowExceeded.TabIndex = 9;
            btnShowExceeded.Text = "Показать превышения";
            btnShowExceeded.Click += btnShowExceeded_Click;
            // 
            // txtResults
            // 
            txtResults.Location = new Point(12, 228);
            txtResults.Margin = new Padding(3, 4, 3, 4);
            txtResults.Multiline = true;
            txtResults.Name = "txtResults";
            txtResults.ReadOnly = true;
            txtResults.ScrollBars = ScrollBars.Vertical;
            txtResults.Size = new Size(902, 532);
            txtResults.TabIndex = 10;
            // 
            // lblDriftStart
            // 
            lblDriftStart.AutoSize = true;
            lblDriftStart.Location = new Point(459, 122);
            lblDriftStart.Name = "lblDriftStart";
            lblDriftStart.Size = new Size(235, 20);
            lblDriftStart.TabIndex = 11;
            lblDriftStart.Text = "Начало диапазона дрейфа (сек):";
            // 
            // numDriftStart
            // 
            numDriftStart.Location = new Point(700, 120);
            numDriftStart.Margin = new Padding(3, 4, 3, 4);
            numDriftStart.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numDriftStart.Name = "numDriftStart";
            numDriftStart.Size = new Size(120, 27);
            numDriftStart.TabIndex = 12;
            numDriftStart.Value = new decimal(new int[] { 1800, 0, 0, 0 });
            // 
            // lblDriftEnd
            // 
            lblDriftEnd.AutoSize = true;
            lblDriftEnd.Location = new Point(459, 174);
            lblDriftEnd.Name = "lblDriftEnd";
            lblDriftEnd.Size = new Size(227, 20);
            lblDriftEnd.TabIndex = 13;
            lblDriftEnd.Text = "Конец диапазона дрейфа (сек):";
            // 
            // numDriftEnd
            // 
            numDriftEnd.Location = new Point(700, 172);
            numDriftEnd.Margin = new Padding(3, 4, 3, 4);
            numDriftEnd.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numDriftEnd.Name = "numDriftEnd";
            numDriftEnd.Size = new Size(120, 27);
            numDriftEnd.TabIndex = 14;
            numDriftEnd.Value = new decimal(new int[] { 3600, 0, 0, 0 });
            // 
            // tabDetectionLimit
            // 
            tabDetectionLimit.Location = new Point(4, 29);
            tabDetectionLimit.Margin = new Padding(3, 4, 3, 4);
            tabDetectionLimit.Name = "tabDetectionLimit";
            tabDetectionLimit.Size = new Size(1162, 991);
            tabDetectionLimit.TabIndex = 1;
            tabDetectionLimit.Text = "Предел детектирования";
            tabDetectionLimit.UseVisualStyleBackColor = true;
            // 
            // tabVirtualSamples
            // 
            tabVirtualSamples.Location = new Point(4, 29);
            tabVirtualSamples.Margin = new Padding(3, 4, 3, 4);
            tabVirtualSamples.Name = "tabVirtualSamples";
            tabVirtualSamples.Size = new Size(1162, 991);
            tabVirtualSamples.TabIndex = 2;
            tabVirtualSamples.Text = "Предел детектирования по виртуальным пробам";
            tabVirtualSamples.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            openFileDialog1.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            openFileDialog1.Title = "Выбор файла для анализа";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1170, 1024);
            Controls.Add(tabControl1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Лабораторный анализ";
            tabControl1.ResumeLayout(false);
            tabRSD.ResumeLayout(false);
            tabRSD.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numMinStdDev).EndInit();
            ((System.ComponentModel.ISupportInitialize)numStartSeconds).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDriftStart).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDriftEnd).EndInit();
            ResumeLayout(false);
        }

        #endregion
    }
}
