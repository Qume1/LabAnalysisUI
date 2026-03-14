#nullable enable
using LabAnalysisUI.Helpers;

namespace LabAnalysisUI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer? components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout = null!;
        private System.Windows.Forms.Panel headerPanel = null!;
        private System.Windows.Forms.Label lblHeaderTitle = null!;
        private System.Windows.Forms.Label lblHeaderSubtitle = null!;
        private System.Windows.Forms.TabControl tabControl1 = null!;
        private System.Windows.Forms.TabPage tabRSD = null!;
        private System.Windows.Forms.TabPage tabDetectionLimit = null!;
        private System.Windows.Forms.TabPage tabVirtualSamples = null!;
        private System.Windows.Forms.OpenFileDialog openFileDialog1 = null!;

        private System.Windows.Forms.Button btnSelectFile = null!;
        private System.Windows.Forms.TextBox txtFilePath = null!;
        private System.Windows.Forms.NumericUpDown numMinStdDev = null!;
        private System.Windows.Forms.NumericUpDown numStartSeconds = null!;
        private System.Windows.Forms.NumericUpDown numDriftStart = null!;
        private System.Windows.Forms.NumericUpDown numDriftEnd = null!;
        private System.Windows.Forms.Button btnAnalyze = null!;
        private System.Windows.Forms.Button btnSaveOutput = null!;
        private System.Windows.Forms.Button btnShowExceeded = null!;
        private System.Windows.Forms.CheckBox chkFilterExceeded = null!;
        private System.Windows.Forms.RichTextBox txtResults = null!;
        private System.Windows.Forms.Label lblNotification = null!;
        private System.Windows.Forms.Label lblRsdSummary = null!;

        private System.Windows.Forms.Button btnDetectionLimitSelectFile = null!;
        private System.Windows.Forms.TextBox txtDetectionLimitFilePath = null!;
        private System.Windows.Forms.Button btnDetectionLimitAnalyze = null!;
        private System.Windows.Forms.Button btnDetectionLimitSave = null!;
        private System.Windows.Forms.RichTextBox txtDetectionLimitResults = null!;
        private System.Windows.Forms.Label lblNotificationDetectionLimit = null!;
        private System.Windows.Forms.Label lblDetectionLimitSummary = null!;

        private System.Windows.Forms.Button btnVirtualSamplesSelectFile = null!;
        private System.Windows.Forms.TextBox txtVirtualSamplesFilePath = null!;
        private System.Windows.Forms.NumericUpDown numCalibrationCoef = null!;
        private System.Windows.Forms.NumericUpDown numIntervalSize = null!;
        private System.Windows.Forms.Button btnVirtualSamplesAnalyze = null!;
        private System.Windows.Forms.Button btnVirtualSamplesSave = null!;
        private System.Windows.Forms.RichTextBox txtVirtualSamplesResults = null!;
        private System.Windows.Forms.Label lblNotificationVirtualSamples = null!;
        private System.Windows.Forms.Label lblVirtualSamplesSummary = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();

            BuildHeader();
            BuildRsdTab();
            BuildDetectionLimitTab();
            BuildVirtualSamplesTab();

            tabControl1 = new System.Windows.Forms.TabControl
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Margin = new System.Windows.Forms.Padding(24, 0, 24, 24)
            };
            tabControl1.Controls.Add(tabRSD);
            tabControl1.Controls.Add(tabDetectionLimit);
            tabControl1.Controls.Add(tabVirtualSamples);
            ThemeHelper.ConfigureTabs(tabControl1);

            rootLayout = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = ThemeHelper.AppBackground
            };
            rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            rootLayout.Controls.Add(headerPanel, 0, 0);
            rootLayout.Controls.Add(tabControl1, 0, 1);

            openFileDialog1.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            openFileDialog1.Title = "Выбор файла для анализа";

            SuspendLayout();
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = ThemeHelper.AppBackground;
            ClientSize = new System.Drawing.Size(1460, 920);
            Controls.Add(rootLayout);
            MinimumSize = new System.Drawing.Size(1180, 760);
            Name = "Form1";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Lab Analysis UI";
            AllowDrop = true;
            DragDrop += Form1_DragDrop;
            DragEnter += Form1_DragEnter;
            ResumeLayout(false);
        }

        private void BuildHeader()
        {
            headerPanel = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Margin = new System.Windows.Forms.Padding(24, 24, 24, 16),
                Padding = new System.Windows.Forms.Padding(28, 22, 28, 18),
                BackColor = ThemeHelper.HeaderBackground
            };
            ThemeHelper.StyleCard(headerPanel);

            var headerLayout = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = ThemeHelper.HeaderBackground
            };
            headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

            lblHeaderTitle = new System.Windows.Forms.Label
            {
                AutoSize = true,
                Text = "Lab Analysis UI",
                Font = new System.Drawing.Font("Segoe UI Semibold", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point),
                ForeColor = ThemeHelper.TextPrimary,
                BackColor = System.Drawing.Color.Transparent,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 8)
            };

            lblHeaderSubtitle = new System.Windows.Forms.Label
            {
                AutoSize = true,
                MaximumSize = new System.Drawing.Size(1180, 0),
                Text = "Обновленное рабочее пространство для расчета СКО, дрейфа и предела детектирования. Перетаскивайте .txt-файлы прямо на форму или активную вкладку.",
                Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point),
                ForeColor = ThemeHelper.TextMuted,
                BackColor = System.Drawing.Color.Transparent
            };

            headerLayout.Controls.Add(lblHeaderTitle, 0, 0);
            headerLayout.Controls.Add(lblHeaderSubtitle, 0, 1);
            headerPanel.Controls.Add(headerLayout);
        }

        private void BuildRsdTab()
        {
            tabRSD = new System.Windows.Forms.TabPage
            {
                Text = "СКО и дрейф",
                Padding = new System.Windows.Forms.Padding(24, 18, 24, 24),
                BackColor = ThemeHelper.AppBackground
            };

            var layout = CreateTabLayout();
            var sidebar = CreateSidebarPanel();
            sidebar.Controls.Add(CreateRsdFileCard());
            sidebar.Controls.Add(CreateRsdSettingsCard());
            sidebar.Controls.Add(CreateRsdActionsCard());

            var resultsCard = CreateResultsCard(
                "Результаты анализа СКО",
                "После запуска здесь появятся общая статистика, расчет дрейфа и перечень превышений.",
                out lblRsdSummary,
                out txtResults);

            layout.Controls.Add(sidebar, 0, 0);
            layout.Controls.Add(resultsCard, 1, 0);
            tabRSD.Controls.Add(layout);
        }

        private void BuildDetectionLimitTab()
        {
            tabDetectionLimit = new System.Windows.Forms.TabPage
            {
                Text = "Предел детектирования",
                Padding = new System.Windows.Forms.Padding(24, 18, 24, 24),
                BackColor = ThemeHelper.AppBackground
            };

            var layout = CreateTabLayout();
            var sidebar = CreateSidebarPanel();
            sidebar.Controls.Add(CreateDetectionFileCard());
            sidebar.Controls.Add(CreateDetectionActionsCard());

            var resultsCard = CreateResultsCard(
                "Результаты предела детектирования",
                "Покажем рассчитанные интервалы, диапазоны и итоговую долю превышений выше порога 0.2.",
                out lblDetectionLimitSummary,
                out txtDetectionLimitResults);

            layout.Controls.Add(sidebar, 0, 0);
            layout.Controls.Add(resultsCard, 1, 0);
            tabDetectionLimit.Controls.Add(layout);
        }

        private void BuildVirtualSamplesTab()
        {
            tabVirtualSamples = new System.Windows.Forms.TabPage
            {
                Text = "Виртуальные пробы",
                Padding = new System.Windows.Forms.Padding(24, 18, 24, 24),
                BackColor = ThemeHelper.AppBackground
            };

            var layout = CreateTabLayout();
            var sidebar = CreateSidebarPanel();
            sidebar.Controls.Add(CreateVirtualSamplesFileCard());
            sidebar.Controls.Add(CreateVirtualSamplesSettingsCard());
            sidebar.Controls.Add(CreateVirtualSamplesActionsCard());

            var resultsCard = CreateResultsCard(
                "Результаты виртуальных проб",
                "В правой панели отобразятся окна виртуальных проб, строки исходного файла и итоговая статистика.",
                out lblVirtualSamplesSummary,
                out txtVirtualSamplesResults);

            layout.Controls.Add(sidebar, 0, 0);
            layout.Controls.Add(resultsCard, 1, 0);
            tabVirtualSamples.Controls.Add(layout);
        }

        private System.Windows.Forms.TableLayoutPanel CreateRsdFileCard()
        {
            var card = CreateSidebarCard();
            AddCardHeader(card, "Файл для анализа", "Выберите текстовый файл с измерениями. Путь можно задать и перетаскиванием.");

            var pickerLayout = CreateFilePickerLayout();
            txtFilePath = new System.Windows.Forms.TextBox
            {
                ReadOnly = true,
                AllowDrop = true,
                Dock = System.Windows.Forms.DockStyle.Fill,
                Margin = new System.Windows.Forms.Padding(0)
            };
            txtFilePath.DragDrop += fileTextBox_DragDrop;
            txtFilePath.DragEnter += fileTextBox_DragEnter;
            ThemeHelper.StyleTextBox(txtFilePath);

            btnSelectFile = new System.Windows.Forms.Button
            {
                Text = "Выбрать файл",
                Size = new System.Drawing.Size(124, 38),
                Margin = new System.Windows.Forms.Padding(12, 0, 0, 0)
            };
            btnSelectFile.Click += btnSelectFile_Click;
            ThemeHelper.StylePrimaryButton(btnSelectFile);

            pickerLayout.Controls.Add(txtFilePath, 0, 0);
            pickerLayout.Controls.Add(btnSelectFile, 1, 0);
            AddCardRow(card, pickerLayout);

            lblNotification = new System.Windows.Forms.Label
            {
                AutoSize = true,
                Text = "Ожидание файла",
                Margin = new System.Windows.Forms.Padding(0, 14, 0, 0)
            };
            ThemeHelper.StyleStatusLabel(lblNotification, ThemeHelper.Warning);
            AddCardRow(card, lblNotification);

            return card;
        }

        private System.Windows.Forms.TableLayoutPanel CreateRsdSettingsCard()
        {
            var card = CreateSidebarCard();
            AddCardHeader(card, "Параметры расчета", "Настройте порог СКО, старт анализа и интервал для вычисления дрейфа.");

            numMinStdDev = CreateNumeric(0.05M, 0M, 10M, 2, 0.01M);
            numStartSeconds = CreateNumeric(1800M, 0M, 100000M, 0, 1M);
            numDriftStart = CreateNumeric(1800M, 0M, 100000M, 0, 1M);
            numDriftEnd = CreateNumeric(3600M, 0M, 100000M, 0, 1M);

            AddCardRow(card, CreateLabeledInput("Порог СКО", numMinStdDev));
            AddCardRow(card, CreateLabeledInput("Начало расчета (сек)", numStartSeconds));
            AddCardRow(card, CreateLabeledInput("Начало дрейфа (сек)", numDriftStart));
            AddCardRow(card, CreateLabeledInput("Конец дрейфа (сек)", numDriftEnd));

            return card;
        }

        private System.Windows.Forms.TableLayoutPanel CreateRsdActionsCard()
        {
            var card = CreateSidebarCard();
            AddCardHeader(card, "Действия", "Enter запускает анализ для активной вкладки. Сохранение работает и без раскрытия превышений в интерфейсе.");

            btnAnalyze = CreateActionButton("Запустить анализ", btnAnalyze_Click, isPrimary: true);
            btnSaveOutput = CreateActionButton("Сохранить отчет", btnSaveOutput_Click, isPrimary: false);
            btnShowExceeded = CreateActionButton("Показать превышения", btnShowExceeded_Click, isPrimary: false);
            chkFilterExceeded = new System.Windows.Forms.CheckBox
            {
                AutoSize = true,
                Text = "Не включать строки с превышениями в файл отчета",
                Margin = new System.Windows.Forms.Padding(0, 10, 0, 0)
            };
            ThemeHelper.StyleCheckBox(chkFilterExceeded);

            AddCardRow(card, btnAnalyze);
            AddCardRow(card, btnShowExceeded);
            AddCardRow(card, btnSaveOutput);
            AddCardRow(card, chkFilterExceeded);
            return card;
        }

        private System.Windows.Forms.TableLayoutPanel CreateDetectionFileCard()
        {
            var card = CreateSidebarCard();
            AddCardHeader(card, "Исходный файл", "Поддерживаются те же текстовые выгрузки. После выбора можно сразу запускать расчет.");

            var pickerLayout = CreateFilePickerLayout();
            txtDetectionLimitFilePath = new System.Windows.Forms.TextBox
            {
                ReadOnly = true,
                AllowDrop = true,
                Dock = System.Windows.Forms.DockStyle.Fill,
                Margin = new System.Windows.Forms.Padding(0)
            };
            txtDetectionLimitFilePath.DragDrop += fileTextBox_DragDrop;
            txtDetectionLimitFilePath.DragEnter += fileTextBox_DragEnter;
            ThemeHelper.StyleTextBox(txtDetectionLimitFilePath);

            btnDetectionLimitSelectFile = new System.Windows.Forms.Button
            {
                Text = "Выбрать файл",
                Size = new System.Drawing.Size(124, 38),
                Margin = new System.Windows.Forms.Padding(12, 0, 0, 0)
            };
            btnDetectionLimitSelectFile.Click += btnDetectionLimitSelectFile_Click;
            ThemeHelper.StylePrimaryButton(btnDetectionLimitSelectFile);

            pickerLayout.Controls.Add(txtDetectionLimitFilePath, 0, 0);
            pickerLayout.Controls.Add(btnDetectionLimitSelectFile, 1, 0);
            AddCardRow(card, pickerLayout);

            lblNotificationDetectionLimit = new System.Windows.Forms.Label
            {
                AutoSize = true,
                Text = "Ожидание файла",
                Margin = new System.Windows.Forms.Padding(0, 14, 0, 0)
            };
            ThemeHelper.StyleStatusLabel(lblNotificationDetectionLimit, ThemeHelper.Warning);
            AddCardRow(card, lblNotificationDetectionLimit);

            return card;
        }

        private System.Windows.Forms.TableLayoutPanel CreateDetectionActionsCard()
        {
            var card = CreateSidebarCard();
            AddCardHeader(card, "Действия", "Порог 0.2 зашит в алгоритм и используется для статистики превышений.");

            btnDetectionLimitAnalyze = CreateActionButton("Рассчитать предел", btnDetectionLimitAnalyze_Click, isPrimary: true);
            btnDetectionLimitSave = CreateActionButton("Сохранить отчет", btnDetectionLimitSave_Click, isPrimary: false);

            AddCardRow(card, btnDetectionLimitAnalyze);
            AddCardRow(card, btnDetectionLimitSave);
            return card;
        }

        private System.Windows.Forms.TableLayoutPanel CreateVirtualSamplesFileCard()
        {
            var card = CreateSidebarCard();
            AddCardHeader(card, "Файл виртуальных проб", "Загрузите текстовый файл, затем уточните коэффициент и размер интервала.");

            var pickerLayout = CreateFilePickerLayout();
            txtVirtualSamplesFilePath = new System.Windows.Forms.TextBox
            {
                ReadOnly = true,
                AllowDrop = true,
                Dock = System.Windows.Forms.DockStyle.Fill,
                Margin = new System.Windows.Forms.Padding(0)
            };
            txtVirtualSamplesFilePath.DragDrop += fileTextBox_DragDrop;
            txtVirtualSamplesFilePath.DragEnter += fileTextBox_DragEnter;
            ThemeHelper.StyleTextBox(txtVirtualSamplesFilePath);

            btnVirtualSamplesSelectFile = new System.Windows.Forms.Button
            {
                Text = "Выбрать файл",
                Size = new System.Drawing.Size(124, 38),
                Margin = new System.Windows.Forms.Padding(12, 0, 0, 0)
            };
            btnVirtualSamplesSelectFile.Click += btnVirtualSamplesSelectFile_Click;
            ThemeHelper.StylePrimaryButton(btnVirtualSamplesSelectFile);

            pickerLayout.Controls.Add(txtVirtualSamplesFilePath, 0, 0);
            pickerLayout.Controls.Add(btnVirtualSamplesSelectFile, 1, 0);
            AddCardRow(card, pickerLayout);

            lblNotificationVirtualSamples = new System.Windows.Forms.Label
            {
                AutoSize = true,
                Text = "Ожидание файла",
                Margin = new System.Windows.Forms.Padding(0, 14, 0, 0)
            };
            ThemeHelper.StyleStatusLabel(lblNotificationVirtualSamples, ThemeHelper.Warning);
            AddCardRow(card, lblNotificationVirtualSamples);

            return card;
        }

        private System.Windows.Forms.TableLayoutPanel CreateVirtualSamplesSettingsCard()
        {
            var card = CreateSidebarCard();
            AddCardHeader(card, "Параметры виртуальных проб", "Используйте калибровочный коэффициент и размер интервала, подходящие для вашей серии измерений.");

            numCalibrationCoef = CreateNumeric(252.1M, 0.1M, 100000M, 1, 0.1M);
            numIntervalSize = CreateNumeric(60M, 1M, 10000M, 0, 1M);

            AddCardRow(card, CreateLabeledInput("Калибровочный коэффициент", numCalibrationCoef));
            AddCardRow(card, CreateLabeledInput("Размер интервала", numIntervalSize));
            return card;
        }

        private System.Windows.Forms.TableLayoutPanel CreateVirtualSamplesActionsCard()
        {
            var card = CreateSidebarCard();
            AddCardHeader(card, "Действия", "Алгоритм рассчитает группы виртуальных проб по пять окон и покажет строки исходных данных.");

            btnVirtualSamplesAnalyze = CreateActionButton("Запустить расчет", btnVirtualSamplesAnalyze_Click, isPrimary: true);
            btnVirtualSamplesSave = CreateActionButton("Сохранить отчет", btnVirtualSamplesSave_Click, isPrimary: false);

            AddCardRow(card, btnVirtualSamplesAnalyze);
            AddCardRow(card, btnVirtualSamplesSave);
            return card;
        }

        private System.Windows.Forms.TableLayoutPanel CreateTabLayout()
        {
            var layout = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = ThemeHelper.AppBackground
            };
            layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 360F));
            layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            return layout;
        }

        private System.Windows.Forms.FlowLayoutPanel CreateSidebarPanel()
        {
            return new System.Windows.Forms.FlowLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                FlowDirection = System.Windows.Forms.FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = ThemeHelper.AppBackground,
                Margin = new System.Windows.Forms.Padding(0, 0, 20, 0),
                Padding = new System.Windows.Forms.Padding(0, 0, 12, 0)
            };
        }

        private System.Windows.Forms.TableLayoutPanel CreateSidebarCard()
        {
            var card = new System.Windows.Forms.TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink,
                Width = 320,
                ColumnCount = 1,
                RowCount = 0,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 18),
                Padding = new System.Windows.Forms.Padding(18),
                BackColor = ThemeHelper.Surface
            };
            card.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            ThemeHelper.StyleCard(card);
            return card;
        }

        private System.Windows.Forms.Panel CreateResultsCard(string title, string description, out System.Windows.Forms.Label summaryLabel, out System.Windows.Forms.RichTextBox resultsBox)
        {
            var card = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Padding = new System.Windows.Forms.Padding(24),
                BackColor = ThemeHelper.Surface,
                Margin = new System.Windows.Forms.Padding(0)
            };
            ThemeHelper.StyleCard(card);

            var layout = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = ThemeHelper.Surface
            };
            layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            var titleLabel = new System.Windows.Forms.Label
            {
                AutoSize = true,
                Text = title,
                Font = new System.Drawing.Font("Segoe UI Semibold", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point),
                ForeColor = ThemeHelper.TextPrimary,
                BackColor = System.Drawing.Color.Transparent,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 6)
            };

            var descriptionLabel = new System.Windows.Forms.Label
            {
                AutoSize = true,
                MaximumSize = new System.Drawing.Size(860, 0),
                Text = description,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point),
                ForeColor = ThemeHelper.TextMuted,
                BackColor = System.Drawing.Color.Transparent,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 18)
            };

            summaryLabel = new System.Windows.Forms.Label
            {
                AutoSize = true,
                MaximumSize = new System.Drawing.Size(860, 0),
                Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point),
                ForeColor = ThemeHelper.TextPrimary,
                BackColor = System.Drawing.Color.Transparent,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 18)
            };

            resultsBox = new System.Windows.Forms.RichTextBox
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ReadOnly = true,
                HideSelection = false,
                ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical,
                Margin = new System.Windows.Forms.Padding(0),
                WordWrap = false
            };
            ThemeHelper.StyleResultBox(resultsBox);

            layout.Controls.Add(titleLabel, 0, 0);
            layout.Controls.Add(descriptionLabel, 0, 1);
            layout.Controls.Add(summaryLabel, 0, 2);
            layout.Controls.Add(resultsBox, 0, 3);
            card.Controls.Add(layout);
            return card;
        }

        private static System.Windows.Forms.TableLayoutPanel CreateFilePickerLayout()
        {
            var layout = new System.Windows.Forms.TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                Dock = System.Windows.Forms.DockStyle.Top,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 0),
                BackColor = System.Drawing.Color.Transparent
            };
            layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            return layout;
        }

        private static System.Windows.Forms.TableLayoutPanel CreateLabeledInput(string labelText, System.Windows.Forms.Control input)
        {
            var layout = new System.Windows.Forms.TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 2,
                Dock = System.Windows.Forms.DockStyle.Top,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 10),
                BackColor = System.Drawing.Color.Transparent
            };
            layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

            var label = new System.Windows.Forms.Label
            {
                AutoSize = true,
                Text = labelText,
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point),
                ForeColor = ThemeHelper.TextMuted,
                BackColor = System.Drawing.Color.Transparent,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 6)
            };

            input.Dock = System.Windows.Forms.DockStyle.Top;
            layout.Controls.Add(label, 0, 0);
            layout.Controls.Add(input, 0, 1);
            return layout;
        }

        private System.Windows.Forms.NumericUpDown CreateNumeric(decimal value, decimal minimum, decimal maximum, int decimalPlaces, decimal increment)
        {
            var numeric = new System.Windows.Forms.NumericUpDown
            {
                Minimum = minimum,
                Maximum = maximum,
                DecimalPlaces = decimalPlaces,
                Increment = increment,
                Dock = System.Windows.Forms.DockStyle.Top,
                Height = 32,
                Margin = new System.Windows.Forms.Padding(0)
            };
            numeric.Value = value;
            ThemeHelper.StyleNumeric(numeric);
            return numeric;
        }

        private System.Windows.Forms.Button CreateActionButton(string text, System.EventHandler clickHandler, bool isPrimary)
        {
            var button = new System.Windows.Forms.Button
            {
                Text = text,
                Width = 282,
                Height = 42,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 10)
            };
            button.Click += clickHandler;

            if (isPrimary)
            {
                ThemeHelper.StylePrimaryButton(button);
            }
            else
            {
                ThemeHelper.StyleSecondaryButton(button);
            }

            return button;
        }

        private static void AddCardHeader(System.Windows.Forms.TableLayoutPanel card, string title, string description)
        {
            var titleLabel = new System.Windows.Forms.Label
            {
                AutoSize = true,
                Text = title,
                Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point),
                ForeColor = ThemeHelper.TextPrimary,
                BackColor = System.Drawing.Color.Transparent,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 6)
            };

            var descriptionLabel = new System.Windows.Forms.Label
            {
                AutoSize = true,
                MaximumSize = new System.Drawing.Size(282, 0),
                Text = description,
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point),
                ForeColor = ThemeHelper.TextMuted,
                BackColor = System.Drawing.Color.Transparent,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 14)
            };

            AddCardRow(card, titleLabel);
            AddCardRow(card, descriptionLabel);
        }

        private static void AddCardRow(System.Windows.Forms.TableLayoutPanel card, System.Windows.Forms.Control control)
        {
            var rowIndex = card.RowCount;
            card.RowCount += 1;
            card.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            card.Controls.Add(control, 0, rowIndex);
        }
    }
}



