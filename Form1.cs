using System.Globalization;
using LabAnalysisUI.Helpers;
using LabAnalysisUI.Models;
using LabAnalysisUI.Services;

namespace LabAnalysisUI
{
    public partial class Form1 : Form
    {
        private readonly FileAnalyzer fileAnalyzer = new();
        private string selectedFilePath = string.Empty;
        private string detectionLimitFilePath = string.Empty;
        private string virtualSamplesFilePath = string.Empty;
        private AnalysisResult? currentResult;
        private DetectionLimitResult? currentDetectionLimitResult;
        private VirtualSamplesResult? currentVirtualSamplesResult;
        private bool isExceededValuesVisible;

        public Form1()
        {
            InitializeComponent();
            ThemeHelper.ConfigureForm(this);
            KeyPreview = true;
            KeyDown += Form1_KeyDown;
            SetupForm();
        }

        private void SetupForm()
        {
            selectedFilePath = string.Empty;
            detectionLimitFilePath = string.Empty;
            virtualSamplesFilePath = string.Empty;
            currentResult = null;
            currentDetectionLimitResult = null;
            currentVirtualSamplesResult = null;
            isExceededValuesVisible = false;

            btnAnalyze.Enabled = false;
            btnSaveOutput.Enabled = false;
            btnShowExceeded.Enabled = false;
            btnDetectionLimitAnalyze.Enabled = false;
            btnDetectionLimitSave.Enabled = false;
            btnVirtualSamplesAnalyze.Enabled = false;
            btnVirtualSamplesSave.Enabled = false;

            txtResults.Clear();
            txtDetectionLimitResults.Clear();
            txtVirtualSamplesResults.Clear();
            chkFilterExceeded.Checked = false;
            UpdateShowExceededButtonText();

            SetStatusLabel(lblNotification, "Ожидание файла", ThemeHelper.Warning);
            SetStatusLabel(lblNotificationDetectionLimit, "Ожидание файла", ThemeHelper.Warning);
            SetStatusLabel(lblNotificationVirtualSamples, "Ожидание файла", ThemeHelper.Warning);

            lblRsdSummary.Text = "Загрузите файл, настройте порог СКО и интервал дрейфа, затем запустите анализ.";
            lblDetectionLimitSummary.Text = "После выбора файла здесь появятся итоговая доля превышений и рассчитанные интервалы.";
            lblVirtualSamplesSummary.Text = "Выберите файл и параметры виртуальных проб, чтобы получить расчет по окнам и диапазонам строк.";
        }

        private void btnSelectFile_Click(object? sender, EventArgs e)
        {
            SelectFile(path => ApplyRsdFile(path, "Файл загружен"));
        }

        private async void btnAnalyze_Click(object? sender, EventArgs e)
        {
            if (!EnsureFileSelected(selectedFilePath))
            {
                return;
            }

            var driftStart = (double)numDriftStart.Value;
            var driftEnd = (double)numDriftEnd.Value;
            if (driftEnd <= driftStart)
            {
                SetStatusLabel(lblNotification, "Проверьте диапазон дрейфа", ThemeHelper.Warning);
                ShowError("Конец диапазона дрейфа должен быть больше начала.");
                return;
            }

            await RunBusyActionAsync(btnAnalyze, "Идет анализ...", async () =>
            {
                currentResult = await Task.Run(() => fileAnalyzer.AnalyzeFile(
                    selectedFilePath,
                    (double)numMinStdDev.Value,
                    (double)numStartSeconds.Value,
                    driftStart,
                    driftEnd));

                if (currentResult is null || !currentResult.IsSuccess)
                {
                    btnSaveOutput.Enabled = false;
                    btnShowExceeded.Enabled = false;
                    txtResults.Clear();
                    lblRsdSummary.Text = currentResult?.Messages.FirstOrDefault() ?? "Не удалось обработать выбранный файл.";
                    SetStatusLabel(lblNotification, "Анализ завершился ошибкой", ThemeHelper.Danger);
                    ShowError(currentResult?.Messages.FirstOrDefault() ?? "Ошибка при анализе файла.");
                    return;
                }

                isExceededValuesVisible = false;
                btnSaveOutput.Enabled = true;
                btnShowExceeded.Enabled = currentResult.ExceededValues.Count > 0;
                UpdateShowExceededButtonText();
                RenderRsdResults();
                SetStatusLabel(lblNotification, "Анализ завершен", ThemeHelper.Success);
            });
        }

        private void btnSaveOutput_Click(object? sender, EventArgs e)
        {
            if (currentResult is null || !currentResult.IsSuccess)
            {
                return;
            }

            var reportLines = FileAnalyzer.CreateAnalysisReport(currentResult, includeExceededValues: !chkFilterExceeded.Checked);
            SaveLines(reportLines, "Сохранить отчет по СКО", "Отчет успешно сохранен.");
        }

        private void btnShowExceeded_Click(object? sender, EventArgs e)
        {
            if (currentResult is null || !currentResult.IsSuccess || currentResult.ExceededValues.Count == 0)
            {
                return;
            }

            var shouldShowExceeded = !isExceededValuesVisible;
            if (shouldShowExceeded && currentResult.ExceededValues.Count > 200)
            {
                var confirmation = MessageBox.Show(
                    "Будет показано более 200 строк. Продолжить?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmation == DialogResult.No)
                {
                    return;
                }
            }

            isExceededValuesVisible = shouldShowExceeded;
            UpdateShowExceededButtonText();
            RenderRsdResults();
        }

        private void btnDetectionLimitSelectFile_Click(object? sender, EventArgs e)
        {
            SelectFile(path => ApplyDetectionLimitFile(path, "Файл загружен"));
        }

        private async void btnDetectionLimitAnalyze_Click(object? sender, EventArgs e)
        {
            if (!EnsureFileSelected(detectionLimitFilePath))
            {
                return;
            }

            await RunBusyActionAsync(btnDetectionLimitAnalyze, "Идет расчет...", async () =>
            {
                currentDetectionLimitResult = await Task.Run(() => fileAnalyzer.AnalyzeDetectionLimit(detectionLimitFilePath));

                if (currentDetectionLimitResult is null || !currentDetectionLimitResult.IsSuccess)
                {
                    btnDetectionLimitSave.Enabled = false;
                    txtDetectionLimitResults.Clear();
                    lblDetectionLimitSummary.Text = currentDetectionLimitResult?.Messages.FirstOrDefault() ?? "Не удалось рассчитать предел детектирования.";
                    SetStatusLabel(lblNotificationDetectionLimit, "Расчет завершился ошибкой", ThemeHelper.Danger);
                    ShowError(currentDetectionLimitResult?.Messages.FirstOrDefault() ?? "Ошибка при анализе файла.");
                    return;
                }

                btnDetectionLimitSave.Enabled = true;
                RenderDetectionLimitResults();
                SetStatusLabel(lblNotificationDetectionLimit, "Расчет завершен", ThemeHelper.Success);
            });
        }

        private void btnDetectionLimitSave_Click(object? sender, EventArgs e)
        {
            if (currentDetectionLimitResult is null || !currentDetectionLimitResult.IsSuccess)
            {
                return;
            }

            SaveLines(currentDetectionLimitResult.Messages, "Сохранить отчет по пределу детектирования", "Отчет успешно сохранен.");
        }

        private void btnVirtualSamplesSelectFile_Click(object? sender, EventArgs e)
        {
            SelectFile(path => ApplyVirtualSamplesFile(path, "Файл загружен"));
        }

        private async void btnVirtualSamplesAnalyze_Click(object? sender, EventArgs e)
        {
            if (!EnsureFileSelected(virtualSamplesFilePath))
            {
                return;
            }

            await RunBusyActionAsync(btnVirtualSamplesAnalyze, "Идет расчет...", async () =>
            {
                currentVirtualSamplesResult = await Task.Run(() => fileAnalyzer.AnalyzeVirtualSamples(
                    virtualSamplesFilePath,
                    (double)numCalibrationCoef.Value,
                    (int)numIntervalSize.Value));

                if (currentVirtualSamplesResult is null || !currentVirtualSamplesResult.IsSuccess)
                {
                    btnVirtualSamplesSave.Enabled = false;
                    txtVirtualSamplesResults.Clear();
                    lblVirtualSamplesSummary.Text = currentVirtualSamplesResult?.Messages.FirstOrDefault() ?? "Не удалось рассчитать виртуальные пробы.";
                    SetStatusLabel(lblNotificationVirtualSamples, "Расчет завершился ошибкой", ThemeHelper.Danger);
                    ShowError(currentVirtualSamplesResult?.Messages.FirstOrDefault() ?? "Ошибка при анализе файла.");
                    return;
                }

                btnVirtualSamplesSave.Enabled = true;
                RenderVirtualSamplesResults();
                SetStatusLabel(lblNotificationVirtualSamples, "Расчет завершен", ThemeHelper.Success);
            });
        }

        private void btnVirtualSamplesSave_Click(object? sender, EventArgs e)
        {
            if (currentVirtualSamplesResult is null || !currentVirtualSamplesResult.IsSuccess)
            {
                return;
            }

            SaveLines(currentVirtualSamplesResult.Messages, "Сохранить отчет по виртуальным пробам", "Отчет успешно сохранен.");
        }

        private void fileTextBox_DragEnter(object? sender, DragEventArgs e)
        {
            e.Effect = TryGetDroppedFile(e, out _) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void fileTextBox_DragDrop(object? sender, DragEventArgs e)
        {
            if (sender is not TextBox textBox || !TryGetDroppedFile(e, out var filePath))
            {
                return;
            }

            if (ReferenceEquals(textBox, txtFilePath))
            {
                ApplyRsdFile(filePath, "Файл добавлен перетаскиванием");
            }
            else if (ReferenceEquals(textBox, txtDetectionLimitFilePath))
            {
                ApplyDetectionLimitFile(filePath, "Файл добавлен перетаскиванием");
            }
            else if (ReferenceEquals(textBox, txtVirtualSamplesFilePath))
            {
                ApplyVirtualSamplesFile(filePath, "Файл добавлен перетаскиванием");
            }
        }

        private void Form1_DragEnter(object? sender, DragEventArgs e)
        {
            e.Effect = TryGetDroppedFile(e, out _) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void Form1_DragDrop(object? sender, DragEventArgs e)
        {
            if (!TryGetDroppedFile(e, out var filePath))
            {
                return;
            }

            if (tabControl1.SelectedTab == tabRSD)
            {
                ApplyRsdFile(filePath, "Файл добавлен перетаскиванием");
            }
            else if (tabControl1.SelectedTab == tabDetectionLimit)
            {
                ApplyDetectionLimitFile(filePath, "Файл добавлен перетаскиванием");
            }
            else if (tabControl1.SelectedTab == tabVirtualSamples)
            {
                ApplyVirtualSamplesFile(filePath, "Файл добавлен перетаскиванием");
            }
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

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

        private void ApplyRsdFile(string filePath, string statusText)
        {
            selectedFilePath = filePath;
            txtFilePath.Text = filePath;
            currentResult = null;
            isExceededValuesVisible = false;
            btnAnalyze.Enabled = true;
            btnSaveOutput.Enabled = false;
            btnShowExceeded.Enabled = false;
            txtResults.Clear();
            UpdateShowExceededButtonText();
            lblRsdSummary.Text = $"Файл {Path.GetFileName(filePath)} готов к анализу. Проверьте параметры слева и запустите расчет.";
            SetStatusLabel(lblNotification, statusText, ThemeHelper.Success);
        }

        private void ApplyDetectionLimitFile(string filePath, string statusText)
        {
            detectionLimitFilePath = filePath;
            txtDetectionLimitFilePath.Text = filePath;
            currentDetectionLimitResult = null;
            btnDetectionLimitAnalyze.Enabled = true;
            btnDetectionLimitSave.Enabled = false;
            txtDetectionLimitResults.Clear();
            lblDetectionLimitSummary.Text = $"Файл {Path.GetFileName(filePath)} готов к расчету предела детектирования.";
            SetStatusLabel(lblNotificationDetectionLimit, statusText, ThemeHelper.Success);
        }

        private void ApplyVirtualSamplesFile(string filePath, string statusText)
        {
            virtualSamplesFilePath = filePath;
            txtVirtualSamplesFilePath.Text = filePath;
            currentVirtualSamplesResult = null;
            btnVirtualSamplesAnalyze.Enabled = true;
            btnVirtualSamplesSave.Enabled = false;
            txtVirtualSamplesResults.Clear();
            lblVirtualSamplesSummary.Text = $"Файл {Path.GetFileName(filePath)} готов к расчету виртуальных проб.";
            SetStatusLabel(lblNotificationVirtualSamples, statusText, ThemeHelper.Success);
        }

        private void RenderRsdResults()
        {
            if (currentResult is null)
            {
                return;
            }

            txtResults.Clear();
            foreach (var line in FileAnalyzer.CreateAnalysisReport(currentResult, isExceededValuesVisible))
            {
                AppendFormattedLine(txtResults, line);
            }

            var exceededCount = currentResult.ExceededValues.Count;
            var driftText = currentResult.MaxSignal.HasValue && currentResult.MinSignal.HasValue
                ? currentResult.DriftValue.ToString("F3", CultureInfo.InvariantCulture)
                : "н/д";
            lblRsdSummary.Text =
                $"Среднее СКО: {currentResult.AverageStdDev:F3} | Превышения: {currentResult.PercentageAboveThreshold:F2}% ({exceededCount} точек)\r\n" +
                $"Длительность: {currentResult.TotalMeasurementTime} сек | Дрейф: {driftText}";
        }

        private void RenderDetectionLimitResults()
        {
            if (currentDetectionLimitResult is null)
            {
                return;
            }

            txtDetectionLimitResults.Clear();
            foreach (var line in currentDetectionLimitResult.Messages)
            {
                AppendFormattedLine(txtDetectionLimitResults, line);
            }

            lblDetectionLimitSummary.Text =
                $"Интервалов: {currentDetectionLimitResult.TotalCount} | Выше 0.2: {currentDetectionLimitResult.CountAboveThreshold}\r\n" +
                $"Доля превышений: {currentDetectionLimitResult.PercentageAboveThreshold:F3}%";
        }

        private void RenderVirtualSamplesResults()
        {
            if (currentVirtualSamplesResult is null)
            {
                return;
            }

            txtVirtualSamplesResults.Clear();
            foreach (var line in currentVirtualSamplesResult.Messages)
            {
                AppendFormattedLine(txtVirtualSamplesResults, line);
            }

            lblVirtualSamplesSummary.Text =
                $"Групп виртуальных проб: {currentVirtualSamplesResult.TotalCount} | Выше 0.2: {currentVirtualSamplesResult.CountAboveThreshold}\r\n" +
                $"Доля превышений: {currentVirtualSamplesResult.PercentageAboveThreshold:F3}%";
        }

        private static void AppendFormattedLine(RichTextBox box, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                box.AppendText(Environment.NewLine);
                return;
            }

            const string driftPrefix = "Значение дрейфа: ";
            if (!text.StartsWith(driftPrefix, StringComparison.Ordinal))
            {
                box.AppendText(text + Environment.NewLine);
                box.SelectionStart = box.TextLength;
                box.ScrollToCaret();
                return;
            }

            var driftValueText = text[driftPrefix.Length..].Trim();
            box.SelectionColor = box.ForeColor;
            box.AppendText(driftPrefix);

            var start = box.TextLength;
            box.AppendText(driftValueText);
            box.Select(start, driftValueText.Length);

            if (TryParseFlexibleDouble(driftValueText, out var driftValue))
            {
                box.SelectionColor = driftValue switch
                {
                    > 30 => ThemeHelper.Danger,
                    > 25 => ThemeHelper.Warning,
                    _ => ThemeHelper.Success
                };
            }
            else
            {
                box.SelectionColor = box.ForeColor;
            }

            box.SelectionStart = box.TextLength;
            box.SelectionColor = box.ForeColor;
            box.AppendText(Environment.NewLine);
            box.ScrollToCaret();
        }

        private async Task RunBusyActionAsync(Button button, string busyText, Func<Task> action)
        {
            var originalText = button.Text;
            try
            {
                button.Enabled = false;
                button.Text = busyText;
                Cursor = Cursors.WaitCursor;
                await action();
            }
            catch (Exception ex)
            {
                ShowError($"Непредвиденная ошибка: {ex.Message}");
            }
            finally
            {
                button.Text = originalText;
                button.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void SaveLines(IEnumerable<string> lines, string title, string successMessage)
        {
            using var saveFileDialog = new SaveFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                Title = title,
                DefaultExt = "txt"
            };

            if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                File.WriteAllLines(saveFileDialog.FileName, lines);
                MessageBox.Show(successMessage, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void SelectFile(Action<string> applySelection)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                applySelection(openFileDialog1.FileName);
            }
        }

        private bool EnsureFileSelected(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                return true;
            }

            ShowError("Сначала выберите файл для анализа.");
            return false;
        }

        private void UpdateShowExceededButtonText()
        {
            btnShowExceeded.Text = isExceededValuesVisible ? "Скрыть превышения" : "Показать превышения";
        }

        private static bool TryGetDroppedFile(DragEventArgs e, out string filePath)
        {
            filePath = string.Empty;
            if (!e.Data!.GetDataPresent(DataFormats.FileDrop))
            {
                return false;
            }

            if (e.Data.GetData(DataFormats.FileDrop) is not string[] files || files.Length == 0)
            {
                return false;
            }

            if (!File.Exists(files[0]))
            {
                return false;
            }

            filePath = files[0];
            return true;
        }

        private static bool TryParseFlexibleDouble(string rawValue, out double value)
        {
            var normalizedValue = rawValue.Replace(',', '.');
            return double.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        private static void SetStatusLabel(Label label, string text, Color accent)
        {
            label.Text = text;
            ThemeHelper.StyleStatusLabel(label, accent);
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

