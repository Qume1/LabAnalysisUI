using LabAnalysisUI.Services;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Timer = System.Windows.Forms.Timer; // Добавлено для устранения неоднозначности


namespace LabAnalysisUI
{
    public partial class Form1 : Form
    {
        private string selectedFilePath;
        private readonly FileAnalyzer fileAnalyzer;
        private FileAnalyzer.AnalysisResult currentResult;
        private bool isExceededValuesVisible = false; // Changed initial state to false
        private string detectionLimitFilePath;
        private FileAnalyzer.DetectionLimitResult currentDetectionLimitResult;
        private string virtualSamplesFilePath;
        private FileAnalyzer.VirtualSamplesResult currentVirtualSamplesResult;
        private Timer notificationTimer;
        private enum NotificationState { FadeIn, Pause, FadeOut, Off }
        private NotificationState notificationState = NotificationState.Off;
        private int notificationAlpha = 0;
        private int pauseTicks = 0;
        private Timer fadeTimer;
        private Color notificationOriginalColor = System.Drawing.Color.Black; // Original notification text color
        private Label currentNotificationLabel;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;                     // Added line to capture key events
            this.KeyDown += new KeyEventHandler(Form1_KeyDown); // Added line for key event
            fileAnalyzer = new FileAnalyzer();
            notificationTimer = new Timer();
            notificationTimer.Interval = 3000; // 3 seconds
            notificationTimer.Tick += NotificationTimer_Tick;
            fadeTimer = new Timer();
            fadeTimer.Interval = 50; // 50 ms per tick for smooth animation
            fadeTimer.Tick += FadeTimer_Tick;
            SetupForm();
        }

        private void SetupForm()
        {
            selectedFilePath = string.Empty;
            detectionLimitFilePath = string.Empty;
            currentDetectionLimitResult = null;
            virtualSamplesFilePath = string.Empty;
            currentVirtualSamplesResult = null;
            UpdateShowExceededButtonText(); // Add initial button text setup
            btnSaveDetectionReport.Enabled = false;  // Add this line
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog1.FileName;
                txtFilePath.Text = selectedFilePath;
                btnAnalyze.Enabled = true;
            }
        }

        private async void btnAnalyze_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Выберите файл для анализа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                btnAnalyze.Enabled = false;
                Cursor = Cursors.WaitCursor;
                txtResults.Clear();

                var startSeconds = (double)numStartSeconds.Value;
                var driftStart = (double)numDriftStart.Value;
                var driftEnd = (double)numDriftEnd.Value;

                if (driftEnd <= driftStart)
                {
                    MessageBox.Show("Конец диапазона дрейфа должен быть больше начала", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                currentResult = await Task.Run(() => fileAnalyzer.AnalyzeFile(
                    selectedFilePath,
                    (double)numMinStdDev.Value,
                    startSeconds,  // Pass startSeconds for СКО calculation
                    driftStart,   // Pass driftStart for drift calculation
                    driftEnd));   // Pass driftEnd for drift calculation

                isExceededValuesVisible = false; // Reset to hidden state after new analysis
                DisplayResults(false); // Initially display without exceeded values

                btnSaveOutput.Enabled = currentResult.IsSuccess;
                btnShowExceeded.Enabled = currentResult.IsSuccess;
                UpdateShowExceededButtonText();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при анализе файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnAnalyze.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void btnSaveOutput_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                saveFileDialog.Title = "Сохранить отчет";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string textToSave = txtResults.Text;

                    // If checkbox is checked, filter the results
                    if (chkFilterExceeded.Checked)
                    {
                        var lines = txtResults.Lines;
                        var filteredLines = new List<string>();
                        decimal threshold = numMinStdDev.Value;

                        foreach (var line in lines)
                        {
                            // Add headers and non-data lines
                            if (!line.Contains("СКО:") || !decimal.TryParse(line.Split(':')[1].Trim(), out decimal sko))
                            {
                                filteredLines.Add(line);
                                continue;
                            }

                            // Only add lines where СКО is within threshold
                            if (sko <= threshold)
                            {
                                filteredLines.Add(line);
                            }
                        }

                        textToSave = string.Join(Environment.NewLine, filteredLines);
                    }

                    File.WriteAllText(saveFileDialog.FileName, textToSave);
                    MessageBox.Show("Отчет сохранен успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnShowExceeded_Click(object sender, EventArgs e)
        {
            if (currentResult == null || !currentResult.IsSuccess)
                return;

            // Determine new visibility state without altering global flag yet.
            bool newVisibility = !isExceededValuesVisible;

            // If enabling exceeded values display and there are more than 200 lines, ask user confirmation.
            if (newVisibility && currentResult.ExceededValues.Count > 200)
            {
                var result = MessageBox.Show("Будет выведено более 200 строк. Продолжить?",
                                             "Предупреждение",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            // Update global flag, button text, and display results.
            isExceededValuesVisible = newVisibility;
            UpdateShowExceededButtonText();
            DisplayResults(isExceededValuesVisible);
        }

        private void UpdateShowExceededButtonText()
        {
            btnShowExceeded.Text = isExceededValuesVisible ? "Скрыть СКО" : "Показать СКО";
        }

        private void DisplayResults(bool showExceededValues)
        {
            if (currentResult == null) return;

            txtResults.Clear();

            // Show general stats
            foreach (var message in currentResult.GeneralStats)
            {
                AppendTextWithScroll(txtResults, message, true);
            }

            // Show exceeded values if enabled
            if (showExceededValues && currentResult.ExceededValues.Any())
            {
                AppendTextWithScroll(txtResults, "");
                AppendTextWithScroll(txtResults, $"Значения СКО, превышающие {currentResult.UsedThreshold:F2}:");
                foreach (var value in currentResult.ExceededValues)
                {
                    AppendTextWithScroll(txtResults, value);
                }
            }
        }

        private void btnDetectionLimitSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                detectionLimitFilePath = openFileDialog1.FileName;
                txtDetectionLimitFilePath.Text = detectionLimitFilePath;
                btnDetectionLimitAnalyze.Enabled = true;
                btnSaveDetectionReport.Enabled = false;  // Add this line
            }
        }

        private async void btnDetectionLimitAnalyze_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(detectionLimitFilePath))
            {
                MessageBox.Show("Выберите файл для анализа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                btnDetectionLimitAnalyze.Enabled = false;
                Cursor = Cursors.WaitCursor;
                txtDetectionLimitResults.Clear();

                currentDetectionLimitResult = await Task.Run(() => fileAnalyzer.AnalyzeDetectionLimit(detectionLimitFilePath));

                if (currentDetectionLimitResult.IsSuccess)
                {
                    foreach (var message in currentDetectionLimitResult.Messages)
                    {
                        AppendTextWithScroll(txtDetectionLimitResults, message, true);
                    }
                    btnDetectionLimitSave.Enabled = true;
                    btnSaveDetectionReport.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Ошибка при анализе файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при анализе файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDetectionLimitAnalyze.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void btnDetectionLimitSave_Click(object sender, EventArgs e)
        {
            if (currentDetectionLimitResult == null || !currentDetectionLimitResult.IsSuccess)
                return;

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                Title = "Сохранить результаты анализа",
                DefaultExt = "txt"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    fileAnalyzer.SaveDetectionLimitResults(currentDetectionLimitResult, saveFileDialog.FileName);
                    MessageBox.Show("Результаты успешно сохранены", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSaveDetectionReport_Click(object sender, EventArgs e)
        {
            if (currentDetectionLimitResult == null || currentDetectionLimitResult.Messages.Count == 0)
            {
                MessageBox.Show("Нет данных для сохранения отчета.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                saveFileDialog.Title = "Сохранить отчет";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        fileAnalyzer.SaveDetectionLimitResults(currentDetectionLimitResult, saveFileDialog.FileName);
                        MessageBox.Show("Отчет успешно сохранен.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при сохранении отчета: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnVirtualSamplesSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                virtualSamplesFilePath = openFileDialog1.FileName;
                txtVirtualSamplesFilePath.Text = virtualSamplesFilePath;
                btnVirtualSamplesAnalyze.Enabled = true;
                btnVirtualSamplesSave.Enabled = false;
            }
        }

        private async void btnVirtualSamplesAnalyze_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(virtualSamplesFilePath))
            {
                MessageBox.Show("Выберите файл для анализа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                btnVirtualSamplesAnalyze.Enabled = false;
                Cursor = Cursors.WaitCursor;
                txtVirtualSamplesResults.Clear();

                double calibrationCoef = (double)numCalibrationCoef.Value;
                int intervalSize = (int)numIntervalSize.Value;

                currentVirtualSamplesResult = await Task.Run(() =>
                    fileAnalyzer.AnalyzeVirtualSamples(virtualSamplesFilePath, calibrationCoef, intervalSize));

                if (currentVirtualSamplesResult.IsSuccess)
                {
                    foreach (var message in currentVirtualSamplesResult.Messages)
                    {
                        AppendTextWithScroll(txtVirtualSamplesResults, message, true);
                    }
                    btnVirtualSamplesSave.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Ошибка при анализе файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при анализе файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnVirtualSamplesAnalyze.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void btnVirtualSamplesSave_Click(object sender, EventArgs e)
        {
            if (currentVirtualSamplesResult == null || !currentVirtualSamplesResult.IsSuccess)
                return;

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                Title = "Сохранить результаты анализа",
                DefaultExt = "txt"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    fileAnalyzer.SaveVirtualSamplesResults(currentVirtualSamplesResult, saveFileDialog.FileName);
                    MessageBox.Show("Результаты успешно сохранены", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtVirtualSamplesFilePath_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblMinStdDev_Click(object sender, EventArgs e)
        {

        }

        // Common DragEnter event handler
        private void fileTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        // Common DragDrop event handler
        private void fileTextBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && File.Exists(files[0]))
            {
                TextBox targetTextBox = sender as TextBox;
                targetTextBox.Text = files[0];

                // Enable corresponding analysis button based on TextBox
                if (targetTextBox == txtFilePath)
                {
                    btnAnalyze.Enabled = true;
                }
                else if (targetTextBox == txtDetectionLimitFilePath)
                {
                    btnDetectionLimitAnalyze.Enabled = true;
                }
                else if (targetTextBox == txtVirtualSamplesFilePath)
                {
                    btnVirtualSamplesAnalyze.Enabled = true;
                }
            }
        }

        // New event handler for DragEnter on the form
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        // New event handler for DragDrop on the form
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0 || !File.Exists(files[0])) return;

            // Determine the active tab and update the corresponding TextBox and file path
            var activeTab = tabControl1.SelectedTab;
            if (activeTab == tabRSD)
            {
                selectedFilePath = files[0];
                txtFilePath.Text = selectedFilePath;
                btnAnalyze.Enabled = true;
            }
            else if (activeTab == tabDetectionLimit)
            {
                detectionLimitFilePath = files[0];
                txtDetectionLimitFilePath.Text = detectionLimitFilePath;
                btnDetectionLimitAnalyze.Enabled = true;
                btnSaveDetectionReport.Enabled = false;
            }
            else if (activeTab == tabVirtualSamples)
            {
                virtualSamplesFilePath = files[0];
                txtVirtualSamplesFilePath.Text = virtualSamplesFilePath;
                btnVirtualSamplesAnalyze.Enabled = true;
            }

            // Display success message after a valid file drop
            StartNotification();
        }

        // New event handler for key down events
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Determine active tab and trigger corresponding Analyze button if enabled
                if (tabControl1.SelectedTab == tabRSD && btnAnalyze.Enabled)
                {
                    btnAnalyze.PerformClick();
                }
                else if (tabControl1.SelectedTab == tabDetectionLimit && btnDetectionLimitAnalyze.Enabled)
                {
                    btnDetectionLimitAnalyze.PerformClick();
                }
                else if (tabControl1.SelectedTab == tabVirtualSamples && btnVirtualSamplesAnalyze.Enabled)
                {
                    btnVirtualSamplesAnalyze.PerformClick();
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void NotificationTimer_Tick(object sender, EventArgs e)
        {
            lblNotification.Visible = false;
            notificationTimer.Stop();
        }

        private void StartNotification()
        {
            currentNotificationLabel = GetNotificationLabel();
            if (currentNotificationLabel == null)
                return;
            currentNotificationLabel.Visible = true;
            notificationAlpha = 0;
            notificationState = NotificationState.FadeIn;
            pauseTicks = 0;
            fadeTimer.Start();
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            const int step = 15; // alpha increment/decrement
            switch (notificationState)
            {
                case NotificationState.FadeIn:
                    notificationAlpha += step;
                    if (notificationAlpha >= 255)
                    {
                        notificationAlpha = 255;
                        notificationState = NotificationState.Pause;
                    }
                    break;
                case NotificationState.Pause:
                    pauseTicks++;
                    if (pauseTicks >= 40) // approx 2000ms pause (40*50ms)
                    {
                        notificationState = NotificationState.FadeOut;
                    }
                    break;
                case NotificationState.FadeOut:
                    notificationAlpha -= step;
                    if (notificationAlpha <= 0)
                    {
                        notificationAlpha = 0;
                        notificationState = NotificationState.Off;
                        if (currentNotificationLabel != null)
                            currentNotificationLabel.Visible = false;
                        fadeTimer.Stop();
                        return;
                    }
                    break;
            }
            if (currentNotificationLabel != null)
                currentNotificationLabel.ForeColor = System.Drawing.Color.FromArgb(notificationAlpha, notificationOriginalColor);
        }

        private Label GetNotificationLabel()
        {
            if (tabControl1.SelectedTab == tabRSD)
                return lblNotification;
            else if (tabControl1.SelectedTab == tabDetectionLimit)
                return lblNotificationDetectionLimit;
            else if (tabControl1.SelectedTab == tabVirtualSamples)
                return lblNotificationVirtualSamples;
            else
                return lblNotification;
        }

        // Add this helper method to standardize text coloring behavior
        private void AppendColoredText(RichTextBox rtb, string message)
        {
            if (message.StartsWith("Значение дрейфа:"))
            {
                string[] parts = message.Split(':');
                if (parts.Length == 2 && double.TryParse(parts[1].Trim(), out double driftValue))
                {
                    rtb.AppendText("Значение дрейфа: ");
                    int startPos = rtb.TextLength;
                    rtb.AppendText($"{driftValue:F2}");
                    int endPos = rtb.TextLength;

                    rtb.Select(startPos, endPos - startPos);
                    if (driftValue > 30)
                    {
                        rtb.SelectionColor = Color.Red;
                    }
                    else if (driftValue > 25)
                    {
                        rtb.SelectionColor = Color.Orange;
                    }
                    rtb.SelectionColor = rtb.ForeColor;
                    rtb.AppendText(Environment.NewLine);
                }
                else
                {
                    rtb.AppendText(message + Environment.NewLine);
                }
            }
            else
            {
                rtb.AppendText(message + Environment.NewLine);
            }
        }

        // Add this helper method right after the other helper methods in the Form1 class
        private void AppendTextWithScroll(RichTextBox rtb, string text, bool colorFormat = false)
        {
            if (colorFormat && text.StartsWith("Значение дрейфа:"))
            {
                string[] parts = text.Split(':');
                if (parts.Length == 2 && double.TryParse(parts[1].Trim(), out double driftValue))
                {
                    rtb.AppendText("Значение дрейфа: ");

                    // Store current position for coloring
                    int startPos = rtb.TextLength;
                    rtb.AppendText($"{driftValue:F2}");
                    int endPos = rtb.TextLength;

                    // Select the numeric part
                    rtb.Select(startPos, endPos - startPos);

                    // Apply color based on value
                    if (driftValue > 30)
                    {
                        rtb.SelectionColor = Color.Red;
                    }
                    else if (driftValue > 25)
                    {
                        rtb.SelectionColor = Color.Orange;
                    }

                    // Reset selection and color
                    rtb.SelectionStart = rtb.TextLength;
                    rtb.SelectionColor = rtb.ForeColor;
                    rtb.AppendText(Environment.NewLine);
                }
                else
                {
                    rtb.AppendText(text + Environment.NewLine);
                }
            }
            else
            {
                rtb.AppendText(text + Environment.NewLine);
            }

            // Ensure scroll to latest text
            rtb.SelectionStart = rtb.TextLength;
            rtb.ScrollToCaret();
        }

    }
}
