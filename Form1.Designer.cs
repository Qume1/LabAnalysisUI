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
        private System.Windows.Forms.CheckBox chkFilterExceeded;

        // New components for Detection Limit tab
        private System.Windows.Forms.Button btnDetectionLimitSelectFile;
        private System.Windows.Forms.TextBox txtDetectionLimitFilePath;
        private System.Windows.Forms.Label lblDetectionLimitFile;
        private System.Windows.Forms.TextBox txtDetectionLimitResults;
        private System.Windows.Forms.Button btnDetectionLimitAnalyze;
        private System.Windows.Forms.Button btnDetectionLimitSave;
        private System.Windows.Forms.Button btnSaveDetectionReport;

        // New components for Virtual Samples tab
        private System.Windows.Forms.Button btnVirtualSamplesSelectFile;
        private System.Windows.Forms.TextBox txtVirtualSamplesFilePath;
        private System.Windows.Forms.Button btnVirtualSamplesAnalyze;
        private System.Windows.Forms.TextBox txtVirtualSamplesResults;
        private System.Windows.Forms.Button btnVirtualSamplesSave;

        // New components for Virtual Samples tab
        
        private System.Windows.Forms.NumericUpDown numCalibrationCoef;
        private System.Windows.Forms.NumericUpDown numIntervalSize;
        private System.Windows.Forms.Label lblCalibrationCoef;
        private System.Windows.Forms.Label lblIntervalSize;

        // New component for notification
        private System.Windows.Forms.Label lblNotification;

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
            if (disposing && (chkFilterExceeded != null))
            {
                chkFilterExceeded.Dispose();
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
            lblNotification = new Label();
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
            chkFilterExceeded = new CheckBox();
            tabDetectionLimit = new TabPage();
            lblDetectionLimitFile = new Label();
            txtDetectionLimitFilePath = new TextBox();
            btnDetectionLimitSelectFile = new Button();
            txtDetectionLimitResults = new TextBox();
            btnDetectionLimitAnalyze = new Button();
            btnDetectionLimitSave = new Button();
            btnSaveDetectionReport = new Button();
            tabVirtualSamples = new TabPage();
            label1 = new Label();
            btnVirtualSamplesSelectFile = new Button();
            txtVirtualSamplesFilePath = new TextBox();
            btnVirtualSamplesAnalyze = new Button();
            txtVirtualSamplesResults = new TextBox();
            btnVirtualSamplesSave = new Button();
            lblCalibrationCoef = new Label();
            numCalibrationCoef = new NumericUpDown();
            lblIntervalSize = new Label();
            numIntervalSize = new NumericUpDown();
            openFileDialog1 = new OpenFileDialog();
            tabControl1.SuspendLayout();
            tabRSD.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numMinStdDev).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numStartSeconds).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDriftStart).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDriftEnd).BeginInit();
            tabDetectionLimit.SuspendLayout();
            tabVirtualSamples.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numCalibrationCoef).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numIntervalSize).BeginInit();
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
            tabRSD.Controls.Add(lblNotification);
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
            tabRSD.Controls.Add(chkFilterExceeded);
            tabRSD.Location = new Point(4, 29);
            tabRSD.Margin = new Padding(3, 4, 3, 4);
            tabRSD.Name = "tabRSD";
            tabRSD.Size = new Size(1162, 991);
            tabRSD.TabIndex = 0;
            tabRSD.Text = "Расчет СКО и дрейфа";
            tabRSD.UseVisualStyleBackColor = true;
            // 
            // lblNotification
            // 
            lblNotification.AutoSize = true;
            lblNotification.BackColor = Color.LightYellow;
            lblNotification.Font = new Font("Segoe UI", 8F);
            lblNotification.ForeColor = Color.Black;
            lblNotification.Location = new Point(212, 29);
            lblNotification.Name = "lblNotification";
            lblNotification.Size = new Size(179, 19);
            lblNotification.TabIndex = 999;
            lblNotification.Text = "Файл успешно скопирован";
            lblNotification.Visible = false;
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
            txtFilePath.AllowDrop = true;
            txtFilePath.Location = new Point(11, 60);
            txtFilePath.Margin = new Padding(3, 4, 3, 4);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.ReadOnly = true;
            txtFilePath.Size = new Size(457, 27);
            txtFilePath.TabIndex = 1;
            txtFilePath.DragDrop += fileTextBox_DragDrop;
            txtFilePath.DragEnter += fileTextBox_DragEnter;
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
            lblMinStdDev.Click += lblMinStdDev_Click;
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
            lblStartSeconds.Location = new Point(50, 174);
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
            btnSaveOutput.Location = new Point(12, 768);
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
            btnShowExceeded.Size = new Size(160, 32);
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
            lblDriftEnd.Location = new Point(467, 174);
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
            // chkFilterExceeded
            // 
            chkFilterExceeded.Location = new Point(155, 772);
            chkFilterExceeded.Name = "chkFilterExceeded";
            chkFilterExceeded.Size = new Size(342, 25);
            chkFilterExceeded.TabIndex = 15;
            chkFilterExceeded.Text = "Не сохранять значения СКО в документ";
            chkFilterExceeded.UseVisualStyleBackColor = true;
            // 
            // tabDetectionLimit
            // 
            tabDetectionLimit.Controls.Add(lblDetectionLimitFile);
            tabDetectionLimit.Controls.Add(txtDetectionLimitFilePath);
            tabDetectionLimit.Controls.Add(btnDetectionLimitSelectFile);
            tabDetectionLimit.Controls.Add(txtDetectionLimitResults);
            tabDetectionLimit.Controls.Add(btnDetectionLimitAnalyze);
            tabDetectionLimit.Controls.Add(btnDetectionLimitSave);
            tabDetectionLimit.Controls.Add(btnSaveDetectionReport);
            tabDetectionLimit.Location = new Point(4, 29);
            tabDetectionLimit.Margin = new Padding(3, 4, 3, 4);
            tabDetectionLimit.Name = "tabDetectionLimit";
            tabDetectionLimit.Size = new Size(1162, 991);
            tabDetectionLimit.TabIndex = 1;
            tabDetectionLimit.Text = "Предел детектирования";
            tabDetectionLimit.UseVisualStyleBackColor = true;
            // 
            // lblDetectionLimitFile
            // 
            lblDetectionLimitFile.AutoSize = true;
            lblDetectionLimitFile.Location = new Point(11, 27);
            lblDetectionLimitFile.Name = "lblDetectionLimitFile";
            lblDetectionLimitFile.Size = new Size(195, 20);
            lblDetectionLimitFile.TabIndex = 0;
            lblDetectionLimitFile.Text = "Выберите текстовый файл:";
            // 
            // txtDetectionLimitFilePath
            // 
            txtDetectionLimitFilePath.AllowDrop = true;
            txtDetectionLimitFilePath.Location = new Point(11, 60);
            txtDetectionLimitFilePath.Margin = new Padding(3, 4, 3, 4);
            txtDetectionLimitFilePath.Name = "txtDetectionLimitFilePath";
            txtDetectionLimitFilePath.ReadOnly = true;
            txtDetectionLimitFilePath.Size = new Size(457, 27);
            txtDetectionLimitFilePath.TabIndex = 1;
            txtDetectionLimitFilePath.DragDrop += fileTextBox_DragDrop;
            txtDetectionLimitFilePath.DragEnter += fileTextBox_DragEnter;
            // 
            // btnDetectionLimitSelectFile
            // 
            btnDetectionLimitSelectFile.Location = new Point(474, 52);
            btnDetectionLimitSelectFile.Margin = new Padding(3, 4, 3, 4);
            btnDetectionLimitSelectFile.Name = "btnDetectionLimitSelectFile";
            btnDetectionLimitSelectFile.Size = new Size(114, 42);
            btnDetectionLimitSelectFile.TabIndex = 2;
            btnDetectionLimitSelectFile.Text = "Обзор...";
            btnDetectionLimitSelectFile.UseVisualStyleBackColor = true;
            btnDetectionLimitSelectFile.Click += btnDetectionLimitSelectFile_Click;
            // 
            // txtDetectionLimitResults
            // 
            txtDetectionLimitResults.Location = new Point(11, 171);
            txtDetectionLimitResults.Margin = new Padding(3, 4, 3, 4);
            txtDetectionLimitResults.Multiline = true;
            txtDetectionLimitResults.Name = "txtDetectionLimitResults";
            txtDetectionLimitResults.ReadOnly = true;
            txtDetectionLimitResults.ScrollBars = ScrollBars.Vertical;
            txtDetectionLimitResults.Size = new Size(902, 532);
            txtDetectionLimitResults.TabIndex = 3;
            // 
            // btnDetectionLimitAnalyze
            // 
            btnDetectionLimitAnalyze.Enabled = false;
            btnDetectionLimitAnalyze.Location = new Point(11, 109);
            btnDetectionLimitAnalyze.Margin = new Padding(3, 4, 3, 4);
            btnDetectionLimitAnalyze.Name = "btnDetectionLimitAnalyze";
            btnDetectionLimitAnalyze.Size = new Size(132, 42);
            btnDetectionLimitAnalyze.TabIndex = 4;
            btnDetectionLimitAnalyze.Text = "Анализировать";
            btnDetectionLimitAnalyze.Click += btnDetectionLimitAnalyze_Click;
            // 
            // btnDetectionLimitSave
            // 
            btnDetectionLimitSave.Enabled = false;
            btnDetectionLimitSave.Location = new Point(11, 670);
            btnDetectionLimitSave.Margin = new Padding(3, 4, 3, 4);
            btnDetectionLimitSave.Name = "btnDetectionLimitSave";
            btnDetectionLimitSave.Size = new Size(137, 33);
            btnDetectionLimitSave.TabIndex = 5;
            btnDetectionLimitSave.Text = "Сохранить отчёт";
            btnDetectionLimitSave.Click += btnDetectionLimitSave_Click;
            // 
            // btnSaveDetectionReport
            // 
            btnSaveDetectionReport.Location = new Point(11, 710);
            btnSaveDetectionReport.Name = "btnSaveDetectionReport";
            btnSaveDetectionReport.Size = new Size(137, 32);
            btnSaveDetectionReport.TabIndex = 0;
            btnSaveDetectionReport.Text = "Сохранить отчёт";
            btnSaveDetectionReport.UseVisualStyleBackColor = true;
            btnSaveDetectionReport.Click += btnSaveDetectionReport_Click;
            // 
            // tabVirtualSamples
            // 
            tabVirtualSamples.Controls.Add(label1);
            tabVirtualSamples.Controls.Add(btnVirtualSamplesSelectFile);
            tabVirtualSamples.Controls.Add(txtVirtualSamplesFilePath);
            tabVirtualSamples.Controls.Add(btnVirtualSamplesAnalyze);
            tabVirtualSamples.Controls.Add(txtVirtualSamplesResults);
            tabVirtualSamples.Controls.Add(btnVirtualSamplesSave);
            tabVirtualSamples.Controls.Add(lblCalibrationCoef);
            tabVirtualSamples.Controls.Add(numCalibrationCoef);
            tabVirtualSamples.Controls.Add(lblIntervalSize);
            tabVirtualSamples.Controls.Add(numIntervalSize);
            tabVirtualSamples.Location = new Point(4, 29);
            tabVirtualSamples.Margin = new Padding(3, 4, 3, 4);
            tabVirtualSamples.Name = "tabVirtualSamples";
            tabVirtualSamples.Size = new Size(1162, 991);
            tabVirtualSamples.TabIndex = 2;
            tabVirtualSamples.Text = "Предел детектирования по виртуальным пробам";
            tabVirtualSamples.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 19);
            label1.Name = "label1";
            label1.Size = new Size(195, 20);
            label1.TabIndex = 5;
            label1.Text = "Выберите текстовый файл:";
            // 
            // btnVirtualSamplesSelectFile
            // 
            btnVirtualSamplesSelectFile.Location = new Point(493, 36);
            btnVirtualSamplesSelectFile.Name = "btnVirtualSamplesSelectFile";
            btnVirtualSamplesSelectFile.Size = new Size(120, 39);
            btnVirtualSamplesSelectFile.TabIndex = 0;
            btnVirtualSamplesSelectFile.Text = "Обзор...";
            btnVirtualSamplesSelectFile.Click += btnVirtualSamplesSelectFile_Click;
            // 
            // txtVirtualSamplesFilePath
            // 
            txtVirtualSamplesFilePath.AllowDrop = true;
            txtVirtualSamplesFilePath.Location = new Point(10, 42);
            txtVirtualSamplesFilePath.Name = "txtVirtualSamplesFilePath";
            txtVirtualSamplesFilePath.ReadOnly = true;
            txtVirtualSamplesFilePath.Size = new Size(477, 27);
            txtVirtualSamplesFilePath.TabIndex = 1;
            txtVirtualSamplesFilePath.TextChanged += txtVirtualSamplesFilePath_TextChanged;
            txtVirtualSamplesFilePath.DragDrop += fileTextBox_DragDrop;
            txtVirtualSamplesFilePath.DragEnter += fileTextBox_DragEnter;
            // 
            // btnVirtualSamplesAnalyze
            // 
            btnVirtualSamplesAnalyze.Enabled = false;
            btnVirtualSamplesAnalyze.Location = new Point(10, 117);
            btnVirtualSamplesAnalyze.Name = "btnVirtualSamplesAnalyze";
            btnVirtualSamplesAnalyze.Size = new Size(132, 33);
            btnVirtualSamplesAnalyze.TabIndex = 2;
            btnVirtualSamplesAnalyze.Text = "Анализировать";
            btnVirtualSamplesAnalyze.Click += btnVirtualSamplesAnalyze_Click;
            // 
            // txtVirtualSamplesResults
            // 
            txtVirtualSamplesResults.Location = new Point(10, 156);
            txtVirtualSamplesResults.Multiline = true;
            txtVirtualSamplesResults.Name = "txtVirtualSamplesResults";
            txtVirtualSamplesResults.ReadOnly = true;
            txtVirtualSamplesResults.ScrollBars = ScrollBars.Vertical;
            txtVirtualSamplesResults.Size = new Size(660, 463);
            txtVirtualSamplesResults.TabIndex = 3;
            // 
            // btnVirtualSamplesSave
            // 
            btnVirtualSamplesSave.Enabled = false;
            btnVirtualSamplesSave.Location = new Point(10, 625);
            btnVirtualSamplesSave.Name = "btnVirtualSamplesSave";
            btnVirtualSamplesSave.Size = new Size(143, 33);
            btnVirtualSamplesSave.TabIndex = 4;
            btnVirtualSamplesSave.Text = "Сохранить отчёт";
            btnVirtualSamplesSave.Click += btnVirtualSamplesSave_Click;
            // 
            // lblCalibrationCoef
            // 
            lblCalibrationCoef.AutoSize = true;
            lblCalibrationCoef.Location = new Point(10, 84);
            lblCalibrationCoef.Name = "lblCalibrationCoef";
            lblCalibrationCoef.Size = new Size(224, 20);
            lblCalibrationCoef.TabIndex = 6;
            lblCalibrationCoef.Text = "Калибровочный коэффициент:";
            // 
            // numCalibrationCoef
            // 
            numCalibrationCoef.DecimalPlaces = 1;
            numCalibrationCoef.Location = new Point(240, 84);
            numCalibrationCoef.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numCalibrationCoef.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numCalibrationCoef.Name = "numCalibrationCoef";
            numCalibrationCoef.Size = new Size(80, 27);
            numCalibrationCoef.TabIndex = 7;
            numCalibrationCoef.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // lblIntervalSize
            // 
            lblIntervalSize.AutoSize = true;
            lblIntervalSize.Location = new Point(347, 86);
            lblIntervalSize.Name = "lblIntervalSize";
            lblIntervalSize.Size = new Size(140, 20);
            lblIntervalSize.TabIndex = 8;
            lblIntervalSize.Text = "Размер интервала:";
            // 
            // numIntervalSize
            // 
            numIntervalSize.Location = new Point(493, 86);
            numIntervalSize.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numIntervalSize.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numIntervalSize.Name = "numIntervalSize";
            numIntervalSize.Size = new Size(60, 27);
            numIntervalSize.TabIndex = 9;
            numIntervalSize.Value = new decimal(new int[] { 60, 0, 0, 0 });
            // 
            // openFileDialog1
            // 
            openFileDialog1.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            openFileDialog1.Title = "Выбор файла для анализа";
            // 
            // Form1
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1170, 1024);
            Controls.Add(tabControl1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Лабораторный анализ";
            DragDrop += Form1_DragDrop;
            DragEnter += Form1_DragEnter;
            tabControl1.ResumeLayout(false);
            tabRSD.ResumeLayout(false);
            tabRSD.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numMinStdDev).EndInit();
            ((System.ComponentModel.ISupportInitialize)numStartSeconds).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDriftStart).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDriftEnd).EndInit();
            tabDetectionLimit.ResumeLayout(false);
            tabDetectionLimit.PerformLayout();
            tabVirtualSamples.ResumeLayout(false);
            tabVirtualSamples.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numCalibrationCoef).EndInit();
            ((System.ComponentModel.ISupportInitialize)numIntervalSize).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
    }
}
