using LabAnalysisUI.Services;
using System.Windows.Forms;
using System.Diagnostics;

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

        public Form1()
        {
            InitializeComponent();
            fileAnalyzer = new FileAnalyzer();
            SetupForm();
        }

        private void SetupForm()
        {
            selectedFilePath = string.Empty;
            detectionLimitFilePath = string.Empty;
            currentDetectionLimitResult = null;
            UpdateShowExceededButtonText(); // Add initial button text setup
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
            if (currentResult == null || !currentResult.IsSuccess)
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
                    fileAnalyzer.SaveResultsToFile(currentResult, saveFileDialog.FileName);
                    MessageBox.Show("Результаты успешно сохранены", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // Показываем общую статистику всегда
            foreach (var message in currentResult.GeneralStats)
            {
                txtResults.AppendText(message + Environment.NewLine);
            }

            // Показываем превышения только если они включены
            if (showExceededValues && currentResult.ExceededValues.Any())
            {
                txtResults.AppendText(Environment.NewLine);
                txtResults.AppendText($"Значения СКО, превышающие {currentResult.UsedThreshold:F2}:" + Environment.NewLine);
                foreach (var value in currentResult.ExceededValues)
                {
                    txtResults.AppendText(value + Environment.NewLine);
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
                        txtDetectionLimitResults.AppendText(message + Environment.NewLine);
                    }
                    btnDetectionLimitSave.Enabled = true;
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
    }
}
