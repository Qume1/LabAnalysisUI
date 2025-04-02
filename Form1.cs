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
        private string virtualSamplesFilePath;
        private FileAnalyzer.VirtualSamplesResult currentVirtualSamplesResult;

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
                MessageBox.Show("�������� ���� ��� �������", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("����� ��������� ������ ������ ���� ������ ������", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                currentResult = await Task.Run(() => fileAnalyzer.AnalyzeFile(
                    selectedFilePath,
                    (double)numMinStdDev.Value,
                    startSeconds,  // Pass startSeconds for ��� calculation
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
                MessageBox.Show($"������ ��� ������� �����: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                saveFileDialog.Filter = "��������� ����� (*.txt)|*.txt|��� ����� (*.*)|*.*";
                saveFileDialog.Title = "��������� �����";

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
                            if (!line.Contains("���:") || !decimal.TryParse(line.Split(':')[1].Trim(), out decimal sko))
                            {
                                filteredLines.Add(line);
                                continue;
                            }

                            // Only add lines where ��� is within threshold
                            if (sko <= threshold)
                            {
                                filteredLines.Add(line);
                            }
                        }

                        textToSave = string.Join(Environment.NewLine, filteredLines);
                    }

                    File.WriteAllText(saveFileDialog.FileName, textToSave);
                    MessageBox.Show("����� �������� �������!", "�����", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                var result = MessageBox.Show("����� �������� ����� 200 �����. ����������?",
                                             "��������������",
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
            btnShowExceeded.Text = isExceededValuesVisible ? "������ ���" : "�������� ���";
        }

        private void DisplayResults(bool showExceededValues)
        {
            if (currentResult == null) return;

            txtResults.Clear();

            // ���������� ����� ���������� ������
            foreach (var message in currentResult.GeneralStats)
            {
                txtResults.AppendText(message + Environment.NewLine);
            }

            // ���������� ���������� ������ ���� ��� ��������
            if (showExceededValues && currentResult.ExceededValues.Any())
            {
                txtResults.AppendText(Environment.NewLine);
                txtResults.AppendText($"�������� ���, ����������� {currentResult.UsedThreshold:F2}:" + Environment.NewLine);
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
                btnSaveDetectionReport.Enabled = false;  // Add this line
            }
        }

        private async void btnDetectionLimitAnalyze_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(detectionLimitFilePath))
            {
                MessageBox.Show("�������� ���� ��� �������", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    btnSaveDetectionReport.Enabled = true;  // Add this line
                }
                else
                {
                    MessageBox.Show("������ ��� ������� �����", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ������� �����: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                Filter = "��������� ����� (*.txt)|*.txt|��� ����� (*.*)|*.*",
                Title = "��������� ���������� �������",
                DefaultExt = "txt"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    fileAnalyzer.SaveDetectionLimitResults(currentDetectionLimitResult, saveFileDialog.FileName);
                    MessageBox.Show("���������� ������� ���������", "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"������ ��� ����������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSaveDetectionReport_Click(object sender, EventArgs e)
        {
            if (currentDetectionLimitResult == null || currentDetectionLimitResult.Messages.Count == 0)
            {
                MessageBox.Show("��� ������ ��� ���������� ������.", "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "��������� ����� (*.txt)|*.txt|��� ����� (*.*)|*.*";
                saveFileDialog.Title = "��������� �����";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        fileAnalyzer.SaveDetectionLimitResults(currentDetectionLimitResult, saveFileDialog.FileName);
                        MessageBox.Show("����� ������� ��������.", "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("������ ��� ���������� ������: " + ex.Message, "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("�������� ���� ��� �������", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        txtVirtualSamplesResults.AppendText(message + Environment.NewLine);
                    }
                    btnVirtualSamplesSave.Enabled = true;
                }
                else
                {
                    MessageBox.Show("������ ��� ������� �����", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ������� �����: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                Filter = "��������� ����� (*.txt)|*.txt|��� ����� (*.*)|*.*",
                Title = "��������� ���������� �������",
                DefaultExt = "txt"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    fileAnalyzer.SaveVirtualSamplesResults(currentVirtualSamplesResult, saveFileDialog.FileName);
                    MessageBox.Show("���������� ������� ���������", "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"������ ��� ����������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtVirtualSamplesFilePath_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblMinStdDev_Click(object sender, EventArgs e)
        {

        }
    }
}
