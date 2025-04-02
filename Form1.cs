using LabAnalysisUI.Services;
using System.Windows.Forms;

namespace LabAnalysisUI
{
    public partial class Form1 : Form
    {
        private string selectedFilePath;
        private readonly FileAnalyzer fileAnalyzer;
        private FileAnalyzer.AnalysisResult currentResult;
        private bool isExceededValuesVisible = false; // Changed initial state to false

        public Form1()
        {
            InitializeComponent();
            fileAnalyzer = new FileAnalyzer();
            SetupForm();
        }

        private void SetupForm()
        {
            selectedFilePath = string.Empty;
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

                currentResult = await Task.Run(() => fileAnalyzer.AnalyzeFile(
                    selectedFilePath,
                    (double)numMinStdDev.Value,
                    (double)numStartSeconds.Value));

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

            isExceededValuesVisible = !isExceededValuesVisible;
            UpdateShowExceededButtonText();
            DisplayResults(isExceededValuesVisible);
        }

        private void UpdateShowExceededButtonText()
        {
            btnShowExceeded.Text = isExceededValuesVisible ? "Скрыть превышения" : "Показать превышения";
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
    }
}
