using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace NppDB.Core
{
    public struct PromptPlaceholder
    {
        public string Name;
    }

    public struct PromptItem
    {
        public string Id;
        public string Title;
        public string Description;
        public string[] Tags;
        public string Text;
        public string Category;
        public PromptPlaceholder[] Placeholders;
    }

    public partial class FrmPromptLibrary : Form
    {

        private static List<PromptItem> _prompts;
        private List<PromptItem> _filteredPrompts;

        private HashSet<string> _placeholdersInCurrentView = new HashSet<string>();
        
        private const int BaseMinPlaceholdersHeight = 80;
        private const int BaseMinPreviewTextHeight = 120;
        private const int BaseLegacyDefaultPlaceholdersHeight = 138;
        private const int BasePlaceholderInputHeight = 92;
        private const int BasePlaceholderGripHeight = 5;
        private const int BaseSearchPanelHeight = 42;
        private const int BasePromptTypeWidth = 190;
        private const int BasePromptGridTagFontDelta = 0;
        private const int BasePromptGridTagPaddingX = 8;
        private const int BasePromptGridTagPaddingY = 4;
        private const int BasePromptGridTagMinHeight = 16;
        private const int BasePromptGridTagMinWidth = 24;
        private const int BasePromptGridTagGap = 6;
        private const int BasePromptGridTagOverflowWidth = 16;
        private const int BasePromptGridLineInset = 6;
        private const int BasePromptGridAccentWidth = 4;
        private const int BasePromptGridInputGap = 4;
        private const int BasePromptGridMinInputWidth = 40;
        private const int BasePromptGridMinContainerWidth = 200;
        private const int BaseSplitterGripWidth = 72;
        private const int BaseWindowMargin = 80;
        private const int BasePreferredWindowWidth = 1500;
        private const int BasePreferredWindowHeight = 900;
        private const int BaseMinimumWindowWidth = 1180;
        private const int BaseMinimumWindowHeight = 680;

        private float _uiScale = 1f;

        private int MinPlaceholdersHeight => ScaleUi(BaseMinPlaceholdersHeight);
        private int MinPreviewTextHeight => ScaleUi(BaseMinPreviewTextHeight);
        private int LegacyDefaultPlaceholdersHeight => ScaleUi(BaseLegacyDefaultPlaceholdersHeight);

        private bool _suppressPromptGridSelectionChanged;

        private readonly ToolTip _actionToolTip = new ToolTip();

        private bool _isEditingTemplate;
        private bool _suppressPromptTextBoxChange;
        private bool _suppressTagsTextBoxChange;
        private bool _suppressPromptMetaTextBoxChange;
        private Timer _templateAutoSaveTimer;
        private PromptItem? _pendingAutoSavePrompt;
        private bool _autoSaveErrorShown;
        private bool _canCopy;
        private string _copyDisabledReason;
        private string _firstMissingPlaceholderName;
        private bool _previewCollapsed;
        private bool _showBlockingValidation;
        private bool _restoreCollapsedAfterEditing;
        private int _lastExpandedPlaceholdersHeight = BaseLegacyDefaultPlaceholdersHeight;
        private bool _isSearchClearHover;
        private bool _suppressCategoryFilterChanged;

        private const int TemplateAutoSaveDebounceMs = 650;

        public static string PromptLibraryPath { get; set; }

        public static Dictionary<string, string> Placeholders;
        
        private Control _resizingControl;
        private int _resizeStartY;

        // TODO: Maybe there is a better way to manage supported placeholders?
        public static readonly List<string> SupportedPlaceholders = new List<string>
        {
            "selected_sql",
            "dialect",
            "table_name",
            "table"
        };

        public FrmPromptLibrary(Dictionary<string, string> placeholders)
        {
            Placeholders = new Dictionary<string, string>(placeholders);

            InitializeComponent();
            _uiScale = GetUiScale();
            _lastExpandedPlaceholdersHeight = LegacyDefaultPlaceholdersHeight;
            UiThemeManager.Register(this);
            
            promptsGridView.CellDoubleClick += promptsGridView_CellDoubleClick; 
            promptsGridView.RowPostPaint += promptsGridView_RowPostPaint;
            promptsGridView.MouseClick += promptsGridView_MouseClick;
            promptTextBox.TextChanged += promptTextBox_TextChanged;
            txtTags.TextChanged += txtTags_TextChanged;
            txtPromptName.TextChanged += txtPromptName_TextChanged;
            txtPromptDescription.TextChanged += txtPromptDescription_TextChanged;
            txtPromptCategory.Leave += txtPromptCategory_Leave;
            panelSearch.Resize += panelSearch_Resize;
            promptsGridView.CellEndEdit += promptsGridView_CellEndEdit;
            promptsGridView.EditingControlShowing += promptsGridView_EditingControlShowing;

            _templateAutoSaveTimer = new Timer { Interval = TemplateAutoSaveDebounceMs };
            _templateAutoSaveTimer.Tick += TemplateAutoSaveTimer_Tick;

            splitterPreview.Paint += splitterPreview_Paint;
            splitterPreview.MouseDoubleClick += splitterPreview_MouseDoubleClick;
            splitterPreview.Cursor = Cursors.HSplit;
            grpPreview.Resize += grpPreview_Resize;
            panelPreviewBottom.Resize += panelPreviewBottom_Resize;
            _actionToolTip.SetToolTip(splitterPreview, "Drag to resize the preview and prompt inputs. Double-click to reset.");
            _actionToolTip.SetToolTip(buttonTogglePreview, "Collapse the preview to focus on prompt inputs.");
            _actionToolTip.SetToolTip(buttonInsertPlaceholder, "Enable Edit, then insert a {{placeholder_name}} token at the caret.");

            ApplyScaledLayoutMetrics();
            UpdateSearchPanelLayout();
            SetEditingMode(false);
            ConfigureCategoryFilter();
            ApplyScaledGridMetrics();
            RefreshPromptList();
        }

        private void ConfigureCategoryFilter()
        {
            cmbPromptSource.Visible = true;
            lblSource.Visible = true;
            lblSource.Text = "Category:";
            panelSearch.Height = ScaleUi(70);
            UpdateSearchPanelLayout();

            colPromptType.Visible = true;
            colPromptType.HeaderText = "Category ↕";

            lblPromptType.Visible = true;
            lblPromptCapabilities.Visible = true;

            lblPromptType.Text = "No database selected";
            lblPromptCapabilities.Text = "No table selected";
            lblPromptType.Width = ScaleUi(BasePromptTypeWidth);

            RefreshCategoryFilter();

            _actionToolTip.SetToolTip(txtSearch, "Search by prompt name, description, or tags. Use tag:... for tag filtering.");
            _actionToolTip.SetToolTip(cmbPromptSource, "Filter prompts by category.");
            _actionToolTip.SetToolTip(buttonDuplicate, "Select a prompt to duplicate.");
            _actionToolTip.SetToolTip(txtTags, "Tip: add the tag 'favorite' to pin a prompt near the top.");
            _actionToolTip.SetToolTip(txtPromptCategory, "Edit the prompt category. Leave the field empty to use 'Custom'.");
        }

        private void UpdateSearchPanelLayout()
        {
            if (txtSearch == null || cmbPromptSource == null)
                return;

            cmbPromptSource.Left = txtSearch.Left;
            cmbPromptSource.Width = txtSearch.Width;
        }

        private void panelSearch_Resize(object sender, EventArgs e)
        {
            UpdateSearchPanelLayout();
        }

        private void RefreshCategoryFilter()
        {
            var previousSelection = cmbPromptSource.SelectedItem as string;
            var categories = (_prompts ?? new List<PromptItem>())
                .Select(GetDisplayCategory)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
                .ToList();

            categories.Insert(0, "All categories");

            _suppressCategoryFilterChanged = true;
            cmbPromptSource.BeginUpdate();
            try
            {
                cmbPromptSource.Items.Clear();
                foreach (var category in categories)
                    cmbPromptSource.Items.Add(category);

                if (!string.IsNullOrWhiteSpace(previousSelection) && categories.Any(c => string.Equals(c, previousSelection, StringComparison.OrdinalIgnoreCase)))
                    cmbPromptSource.SelectedItem = categories.First(c => string.Equals(c, previousSelection, StringComparison.OrdinalIgnoreCase));
                else if (cmbPromptSource.Items.Count > 0)
                    cmbPromptSource.SelectedIndex = 0;
            }
            finally
            {
                cmbPromptSource.EndUpdate();
                _suppressCategoryFilterChanged = false;
            }
        }

        private void ApplyScaledGridMetrics()
        {
            promptsGridView.ColumnHeadersHeight = ScaleUi(30);
            promptsGridView.RowTemplate.Height = ScaleUi(58);
            promptsGridView.RowTemplate.DividerHeight = 0;
            if (colPromptName != null)
                colPromptName.Width = ScaleUi(170);
            if (colPromptType != null)
                colPromptType.Width = ScaleUi(95);
        }

        private int MeasureSingleLineHeight(Font font, int verticalPadding)
        {
            return Math.Max(ScaleUi(18), TextRenderer.MeasureText("Ag", font).Height + verticalPadding);
        }

        private int MeasureTextWidth(string text, Font font, int horizontalPadding)
        {
            var measured = TextRenderer.MeasureText(text ?? string.Empty, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
            return Math.Max(ScaleUi(24), measured.Width + horizontalPadding);
        }

        private void ApplyScaledLayoutMetrics()
        {
            var metaHeight = MeasureSingleLineHeight(lblEditingBadge.Font, ScaleUi(8));
            var workflowHeight = MeasureSingleLineHeight(lblWorkflowHint.Font, ScaleUi(6));
            var tagsHeight = MeasureSingleLineHeight(txtTags.Font, ScaleUi(10));
            var placeholderHeaderHeight = MeasureSingleLineHeight(lblPlaceholders.Font, ScaleUi(6));
            var actionButtonHeight = MeasureSingleLineHeight(buttonAdd.Font, ScaleUi(12));

            panelPromptMeta.Height = metaHeight;
            panelMetaRight.Height = metaHeight;
            lblPromptType.Height = metaHeight;
            lblPromptCapabilities.Height = metaHeight;

            lblEditingBadge.AutoSize = false;
            lblEditingBadge.Padding = new Padding(ScaleUi(6), 0, ScaleUi(6), 0);
            lblEditingBadge.Width = MeasureTextWidth(lblEditingBadge.Text, lblEditingBadge.Font, ScaleUi(18));
            lblEditingBadge.Height = metaHeight;

            editingModeCheckbox.Width = MeasureTextWidth(editingModeCheckbox.Text, editingModeCheckbox.Font, ScaleUi(24));
            editingModeCheckbox.Height = metaHeight;

            buttonTogglePreview.Width = MeasureTextWidth(buttonTogglePreview.Text, buttonTogglePreview.Font, ScaleUi(24));
            buttonTogglePreview.Height = metaHeight;

            buttonInsertPlaceholder.Width = MeasureTextWidth(buttonInsertPlaceholder.Text, buttonInsertPlaceholder.Font, ScaleUi(24));
            buttonInsertPlaceholder.Height = placeholderHeaderHeight;

            buttonAiStudio.Width = MeasureTextWidth(buttonAiStudio.Text, buttonAiStudio.Font, ScaleUi(24));
            buttonAiStudio.Height = metaHeight;

            panelMetaRight.Width = lblEditingBadge.Width + editingModeCheckbox.Width + buttonTogglePreview.Width + buttonAiStudio.Width;

            panelWorkflowHint.Padding = new Padding(0, ScaleUi(3), 0, ScaleUi(1));
            panelWorkflowHint.Height = workflowHeight + panelWorkflowHint.Padding.Top + panelWorkflowHint.Padding.Bottom;
            lblWorkflowHint.Height = workflowHeight;

            panelPromptTags.Height = tagsHeight + ScaleUi(12);
            lblTags.Location = new Point(0, ScaleUi(7));
            txtTags.Location = new Point(ScaleUi(47), ScaleUi(7));
            txtTags.Height = tagsHeight;

            var editFieldHeight = MeasureSingleLineHeight(txtPromptName.Font, ScaleUi(10));
            var editFieldLabelLeft = 0;
            var editFieldValueLeft = ScaleUi(78);
            var editFieldTop = ScaleUi(3);
            var editFieldRowGap = ScaleUi(6);
            var editFieldLabelOffset = Math.Max(0, (editFieldHeight - lblPromptName.Height) / 2);

            lblPromptName.Location = new Point(editFieldLabelLeft, editFieldTop + editFieldLabelOffset);
            txtPromptName.Location = new Point(editFieldValueLeft, editFieldTop);
            txtPromptName.Height = editFieldHeight;

            var descriptionTop = txtPromptName.Bottom + editFieldRowGap;
            lblPromptDescription.Location = new Point(editFieldLabelLeft, descriptionTop + editFieldLabelOffset);
            txtPromptDescription.Location = new Point(editFieldValueLeft, descriptionTop);
            txtPromptDescription.Height = editFieldHeight;

            var categoryTop = txtPromptDescription.Bottom + editFieldRowGap;
            lblPromptCategory.Location = new Point(editFieldLabelLeft, categoryTop + editFieldLabelOffset);
            txtPromptCategory.Location = new Point(editFieldValueLeft, categoryTop);
            txtPromptCategory.Height = editFieldHeight;

            panelEditFields.Height = txtPromptCategory.Bottom + ScaleUi(7);

            panelLeftActions.Padding = new Padding(0, ScaleUi(8), 0, 0);
            panelLeftActions.Height = actionButtonHeight + panelLeftActions.Padding.Top + ScaleUi(10);
            buttonAdd.Height = actionButtonHeight;
            buttonDuplicate.Height = actionButtonHeight;
            buttonDelete.Height = actionButtonHeight;

            panelRightActions.Padding = new Padding(ScaleUi(6));
            panelRightActions.Height = actionButtonHeight + panelRightActions.Padding.Vertical;
            buttonCopy.Height = actionButtonHeight;

            lblPlaceholders.AutoSize = false;
            lblPlaceholders.Location = new Point(ScaleUi(3), ScaleUi(3));
            lblPlaceholders.Height = placeholderHeaderHeight;

            buttonInsertPlaceholder.Location = new Point(
                Math.Max(ScaleUi(3), panelPreviewBottom.ClientSize.Width - buttonInsertPlaceholder.Width - ScaleUi(3)),
                ScaleUi(1));

            var placeholderLabelRight = buttonInsertPlaceholder.Left - ScaleUi(6);
            lblPlaceholders.Width = Math.Max(ScaleUi(220), placeholderLabelRight - lblPlaceholders.Left);

            var placeholderHeaderBottom = Math.Max(lblPlaceholders.Bottom, buttonInsertPlaceholder.Bottom);
            flowLayoutPanelPlaceholders.Location = new Point(ScaleUi(3), placeholderHeaderBottom + ScaleUi(4));
            flowLayoutPanelPlaceholders.MinimumSize = new Size(ScaleUi(200), MinPlaceholdersHeight);
            flowLayoutPanelPlaceholders.Padding = new Padding(ScaleUi(6));
            flowLayoutPanelPlaceholders.Size = new Size(
                Math.Max(ScaleUi(200), panelPreviewBottom.ClientSize.Width - ScaleUi(6)),
                Math.Max(MinPlaceholdersHeight, panelPreviewBottom.ClientSize.Height - flowLayoutPanelPlaceholders.Top - ScaleUi(3)));
        }

        private void panelPreviewBottom_Resize(object sender, EventArgs e)
        {
            ApplyScaledLayoutMetrics();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RestoreLayoutFromSettings();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            FlushPendingAutoSave();
            SaveLayoutToSettings();
            base.OnFormClosing(e);
        }

        private float GetUiScale()
        {
            try
            {
                using (var graphics = CreateGraphics())
                {
                    var scale = graphics.DpiY / 96f;
                    return scale > 0.1f ? scale : 1f;
                }
            }
            catch
            {
                return 1f;
            }
        }

        private int ScaleUi(int value)
        {
            return Math.Max(1, (int)Math.Round(value * _uiScale));
        }


        private void RestoreLayoutFromSettings()
        {
            var workingArea = Screen.FromControl(this).WorkingArea;
            var defaultSize = GetDefaultPromptLibrarySize(workingArea);

            MinimumSize = defaultSize;

            var rawSize = Properties.Settings.Default["PromptLibrary_Size"];
            var savedSize = rawSize is Size s ? s : Size.Empty;
            var hasSavedSize = savedSize.Width > 0 && savedSize.Height > 0;

            Size = hasSavedSize
                ? ClampSizeToWorkingArea(savedSize, workingArea)
                : defaultSize;

            var rawLocation = Properties.Settings.Default["PromptLibrary_Location"];
            var savedLocation = rawLocation is Point p ? p : Point.Empty;

            StartPosition = FormStartPosition.Manual;
            Location = !savedLocation.IsEmpty
                ? ClampLocationToWorkingArea(savedLocation, Size, workingArea)
                : GetCenteredLocation(workingArea, Size);

            var rawHeight = Properties.Settings.Default["PromptLibrary_PlaceholdersHeight"];
            var savedPlaceholdersHeight = rawHeight is int h && h > 0 ? h : BaseLegacyDefaultPlaceholdersHeight;
            var desiredPlaceholdersHeight = !hasSavedSize
                ? Math.Max(GetDefaultPlaceholdersHeight(), ScaleUi(savedPlaceholdersHeight))
                : savedPlaceholdersHeight;

            ApplyPreviewBottomHeight(desiredPlaceholdersHeight);
            _lastExpandedPlaceholdersHeight = panelPreviewBottom.Height;

            var rawPreviewCollapsed = Properties.Settings.Default["PromptLibrary_PreviewCollapsed"];
            var savedPreviewCollapsed = rawPreviewCollapsed is bool b && b;
            SetPreviewCollapsed(savedPreviewCollapsed, false);
            ApplyPreviewSplitterVisuals();
        }

        private void SaveLayoutToSettings()
        {
            if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.PromptLibrary_Size = Size;
                Properties.Settings.Default.PromptLibrary_Location = Location;
            }
            else
            {
                Properties.Settings.Default.PromptLibrary_Size = RestoreBounds.Size;
                Properties.Settings.Default.PromptLibrary_Location = RestoreBounds.Location;
            }

            Properties.Settings.Default.PromptLibrary_PlaceholdersHeight = _previewCollapsed
                ? _lastExpandedPlaceholdersHeight
                : panelPreviewBottom.Height;
            Properties.Settings.Default.PromptLibrary_PreviewCollapsed = _previewCollapsed;
            Properties.Settings.Default.Save();
        }

        private Size GetDefaultPromptLibrarySize(Rectangle workingArea)
        {
            var minWidth = ScaleUi(BaseMinimumWindowWidth);
            var minHeight = ScaleUi(BaseMinimumWindowHeight);
            var preferredWidth = Math.Min(ScaleUi(BasePreferredWindowWidth), Math.Max(minWidth, workingArea.Width - ScaleUi(BaseWindowMargin)));
            var preferredHeight = Math.Min(ScaleUi(BasePreferredWindowHeight), Math.Max(minHeight, workingArea.Height - ScaleUi(BaseWindowMargin)));

            preferredWidth = Math.Min(preferredWidth, workingArea.Width);
            preferredHeight = Math.Min(preferredHeight, workingArea.Height);

            return new Size(
                Math.Max(MinimumSize.Width, preferredWidth),
                Math.Max(MinimumSize.Height, preferredHeight));
        }

        private Size ClampSizeToWorkingArea(Size size, Rectangle workingArea)
        {
            return new Size(
                Math.Max(MinimumSize.Width, Math.Min(size.Width, workingArea.Width)),
                Math.Max(MinimumSize.Height, Math.Min(size.Height, workingArea.Height)));
        }

        private static Point ClampLocationToWorkingArea(Point location, Size size, Rectangle workingArea)
        {
            var maxX = Math.Max(workingArea.Left, workingArea.Right - size.Width);
            var maxY = Math.Max(workingArea.Top, workingArea.Bottom - size.Height);

            return new Point(
                Math.Min(Math.Max(location.X, workingArea.Left), maxX),
                Math.Min(Math.Max(location.Y, workingArea.Top), maxY));
        }

        private static Point GetCenteredLocation(Rectangle workingArea, Size size)
        {
            return new Point(
                workingArea.Left + Math.Max(0, (workingArea.Width - size.Width) / 2),
                workingArea.Top + Math.Max(0, (workingArea.Height - size.Height) / 2));
        }

        private int GetDefaultPlaceholdersHeight()
        {
            var preferredHeight = Math.Max(ScaleUi(250), grpPreview.ClientSize.Height * 46 / 100);
            return Math.Min(ScaleUi(380), preferredHeight);
        }

        private void ApplyPreviewBottomHeight(int desiredHeight)
        {
            var maxBottom = Math.Max(MinPlaceholdersHeight, grpPreview.ClientSize.Height - MinPreviewTextHeight);
            panelPreviewBottom.Height = Math.Max(MinPlaceholdersHeight, Math.Min(desiredHeight, maxBottom));
        }

        private int GetCollapsedPlaceholdersHeight()
        {
            var displayHeight = Math.Max(0, grpPreview.DisplayRectangle.Height);
            var reservedHeight = panelPromptMeta.Visible ? panelPromptMeta.Height : 0;
            if (panelPromptTags.Visible)
                reservedHeight += panelPromptTags.Height;

            return Math.Max(MinPlaceholdersHeight, displayHeight - reservedHeight);
        }


        private void ApplyPreviewSplitterVisuals()
        {
            var pal = UiThemeManager.Current;
            splitterPreview.BackColor = pal.IsDark
                ? BlendColor(pal.PureBackground, Color.White, 0.12f)
                : BlendColor(SystemColors.ControlLight, Color.Black, 0.10f);
        }

        private void UpdatePreviewToggleState()
        {
            buttonTogglePreview.Text = _previewCollapsed ? "Show Preview" : "Collapse Preview";
            ApplyScaledLayoutMetrics();
            _actionToolTip.SetToolTip(buttonTogglePreview,
                _previewCollapsed
                    ? "Show the prompt preview again."
                    : "Collapse the preview to focus on prompt inputs.");
        }

        private void SetPreviewCollapsed(bool collapsed, bool rememberCurrentHeight)
        {
            if (rememberCurrentHeight && !_previewCollapsed && collapsed)
                _lastExpandedPlaceholdersHeight = panelPreviewBottom.Height;

            grpPreview.SuspendLayout();

            if (collapsed)
            {
                promptTextBox.Visible = false;
                panelPromptTags.Visible = false;
                splitterPreview.Visible = false;
                panelPreviewBottom.Height = GetCollapsedPlaceholdersHeight();
            }
            else
            {
                promptTextBox.Visible = true;
                panelPromptTags.Visible = true;
                splitterPreview.Visible = true;

                ApplyPreviewBottomHeight(_lastExpandedPlaceholdersHeight > 0
                    ? _lastExpandedPlaceholdersHeight
                    : GetDefaultPlaceholdersHeight());
            }

            _previewCollapsed = collapsed;
            UpdatePreviewToggleState();
            grpPreview.ResumeLayout(true);
            grpPreview.PerformLayout();
        }

        private static bool SupportsAutoFill(string placeholderName)
        {
            return string.Equals(placeholderName, "selected_sql", StringComparison.OrdinalIgnoreCase)
                   || string.Equals(placeholderName, "dialect", StringComparison.OrdinalIgnoreCase)
                   || string.Equals(placeholderName, "table_name", StringComparison.OrdinalIgnoreCase)
                   || string.Equals(placeholderName, "table", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetPlaceholderDisplayName(string placeholderName)
        {
            if (string.IsNullOrWhiteSpace(placeholderName))
                return "Value";

            switch (placeholderName.Trim().ToLowerInvariant())
            {
                case "selected_sql":
                    return "Selected SQL Query";
                case "dialect":
                    return "Current SQL Dialect";
                case "target_dialect":
                    return "Target SQL Dialect";
                case "source_dialect":
                    return "Source SQL Dialect";
                case "table":
                    return "Selected Tables' Metadata";
                case "table_name":
                    return "Selected Tables";
                case "issue_description":
                    return "Issue Description";
            }

            var parts = placeholderName
                .Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part =>
                {
                    switch (part.ToLowerInvariant())
                    {
                        case "sql": return "SQL";
                        case "db": return "DB";
                        case "llm": return "LLM";
                        case "api": return "API";
                        case "id": return "ID";
                        case "json": return "JSON";
                        case "csv": return "CSV";
                        case "rest": return "REST";
                        default:
                            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(part.ToLowerInvariant());
                    }
                });

            return string.Join(" ", parts);
        }

        private static string GetPlaceholderManualInstruction(string placeholderName)
        {
            if (string.IsNullOrWhiteSpace(placeholderName))
                return "Enter the value here.";

            if (string.Equals(placeholderName, "selected_sql", StringComparison.OrdinalIgnoreCase))
                return "Select SQL code in the Notepad++ editor to auto-fill, or type it manually.";

            if (string.Equals(placeholderName, "dialect", StringComparison.OrdinalIgnoreCase))
                return "Auto-fills from the active database connection, or type it manually.";

            if (string.Equals(placeholderName, "table_name", StringComparison.OrdinalIgnoreCase))
                return "Select a table in the DB Connect Manager to auto-fill, or type it manually.";

            if (string.Equals(placeholderName, "table", StringComparison.OrdinalIgnoreCase))
                return "Select a table in the DB Connect Manager to auto-fill, or enter the metadata manually.";

            if (placeholderName.IndexOf("query", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Paste or type the query here.";

            if (placeholderName.IndexOf("dialect", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Enter the SQL dialect here.";

            if (placeholderName.IndexOf("description", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Describe the issue or request here.";

            if (placeholderName.EndsWith("_name", StringComparison.OrdinalIgnoreCase)
                || placeholderName.IndexOf("name", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Enter the name here.";

            return "Enter the value here.";
        }

        private static string GetAutoFillBadgeText(string placeholderName)
        {
            return SupportsAutoFill(placeholderName) ? "AUTO-FILL" : "MANUAL";
        }

        private static string GetAutoFillTooltip(string placeholderName)
        {
            if (!SupportsAutoFill(placeholderName))
                return "Type this value manually.";

            if (string.Equals(placeholderName, "selected_sql", StringComparison.OrdinalIgnoreCase))
                return "Can auto-fill from the selected SQL in the Notepad++ editor.";

            if (string.Equals(placeholderName, "dialect", StringComparison.OrdinalIgnoreCase))
                return "Can auto-fill from the active database connection.";

            if (string.Equals(placeholderName, "table_name", StringComparison.OrdinalIgnoreCase))
                return "Can auto-fill from the selected table in the DB Connect Manager.";

            if (string.Equals(placeholderName, "table", StringComparison.OrdinalIgnoreCase))
                return "Can auto-fill from the selected table metadata in the DB Connect Manager.";

            return "This field supports auto-fill when the required editor or DB context is available.";
        }

        private Color GetPlaceholderHintForeColor()
        {
            var pal = UiThemeManager.Current;
            return pal.IsDark ? Color.FromArgb(205, 210, 216) : Color.FromArgb(89, 89, 89);
        }

        private Label CreatePlaceholderBadge(string name, string text)
        {
            return new Label
            {
                AutoSize = true,
                Margin = new Padding(0, 0, ScaleUi(8), 0),
                Padding = new Padding(ScaleUi(6), ScaleUi(2), ScaleUi(6), ScaleUi(2)),
                Name = name,
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
        }

        private void ApplyPlaceholderBadgeState(Label badge, string text, Color backColor, Color foreColor, bool visible = true)
        {
            if (badge == null)
                return;

            badge.Text = text;
            badge.BackColor = backColor;
            badge.ForeColor = foreColor;
            badge.Visible = visible;
        }

        private void UpdatePlaceholderBadges(FlowLayoutPanel container, bool hasValue, bool showBlockingState)
        {
            var pal = UiThemeManager.Current;
            var placeholderName = container.Tag as string;
            var modeBadge = container.Controls.Find("lblFieldModeBadge", true).OfType<Label>().FirstOrDefault();
            var valueBadge = container.Controls.Find("lblFieldValueBadge", true).OfType<Label>().FirstOrDefault();
            var valueBadgeToolTipMissing = "This input is required but it is not filled in yet.";
            var valueBadgeToolTipReady = "This input has a value and is ready to use.";

            var autoBack = pal.IsDark ? Color.FromArgb(34, 58, 84) : Color.FromArgb(229, 242, 255);
            var autoFore = pal.IsDark ? Color.FromArgb(205, 227, 255) : Color.FromArgb(20, 78, 145);
            var manualBack = pal.IsDark ? Color.FromArgb(62, 62, 62) : Color.FromArgb(228, 232, 237);
            var manualFore = pal.IsDark ? pal.Text : Color.FromArgb(68, 68, 68);
            var readyBack = pal.IsDark ? Color.FromArgb(38, 74, 52) : Color.FromArgb(231, 247, 236);
            var readyFore = pal.IsDark ? Color.FromArgb(209, 245, 219) : Color.FromArgb(26, 102, 56);
            var missingBack = pal.IsDark ? Color.FromArgb(90, 55, 38) : Color.FromArgb(255, 244, 224);
            var missingFore = pal.IsDark ? Color.FromArgb(255, 224, 186) : Color.FromArgb(140, 88, 15);

            ApplyPlaceholderBadgeState(modeBadge, GetAutoFillBadgeText(placeholderName),
                SupportsAutoFill(placeholderName) ? autoBack : manualBack,
                SupportsAutoFill(placeholderName) ? autoFore : manualFore);

            if (hasValue)
            {
                ApplyPlaceholderBadgeState(valueBadge, "READY", readyBack, readyFore);
                _actionToolTip.SetToolTip(valueBadge, valueBadgeToolTipReady);
                return;
            }

            ApplyPlaceholderBadgeState(valueBadge, "MISSING", missingBack, missingFore);
            _actionToolTip.SetToolTip(valueBadge, valueBadgeToolTipMissing);
        }
        
        private static string GetPlaceholderValue(string key)
        {
            if (Placeholders == null)
                return string.Empty;

            return Placeholders.TryGetValue(key, out var value)
                ? value ?? string.Empty
                : string.Empty;
        }

        public static void SetPrompts(List<PromptItem> promptItems)
        {
            _prompts = promptItems;
        }

        private static string[] ParseTags(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return Array.Empty<string>();

            return raw
                .Split(new[] { ',', ';', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => (t ?? string.Empty).Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static string FormatTags(string[] tags)
        {
            if (tags == null || tags.Length == 0)
                return string.Empty;

            return string.Join(", ", tags.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()));
        }

        private static string[] GetVisibleTags(string[] tags)
        {
            if (tags == null)
                return Array.Empty<string>();

            return tags
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .ToArray();
        }

        private static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();

            if (rect.Width <= 0 || rect.Height <= 0)
                return path;

            var diameter = Math.Max(2, radius * 2);
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static bool ContainsIgnoreCase(string haystack, string needle)
        {
            if (string.IsNullOrEmpty(needle))
                return true;
            if (string.IsNullOrEmpty(haystack))
                return false;
            return haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string GetDisplayCategory(PromptItem prompt)
        {
            var category = (prompt.Category ?? string.Empty).Trim();
            return string.IsNullOrWhiteSpace(category) ? "General" : category;
        }

        private static bool IsFavoritePrompt(PromptItem prompt)
        {
            var tags = prompt.Tags ?? Array.Empty<string>();
            return tags.Any(t =>
                string.Equals((t ?? string.Empty).Trim(), "favorite", StringComparison.OrdinalIgnoreCase)
                || string.Equals((t ?? string.Empty).Trim(), "favourite", StringComparison.OrdinalIgnoreCase)
                || string.Equals((t ?? string.Empty).Trim(), "pinned", StringComparison.OrdinalIgnoreCase)
                || string.Equals((t ?? string.Empty).Trim(), "starred", StringComparison.OrdinalIgnoreCase));
        }

        private static string GetPromptDisplayTitle(PromptItem prompt)
        {
            var title = (prompt.Title ?? string.Empty).Trim();
            return IsFavoritePrompt(prompt) ? "★ " + title : title;
        }

        private static string NormalizePromptTitle(string title)
        {
            return (title ?? string.Empty).Trim().ToLowerInvariant();
        }

        private static int GetDefaultPriorityRank(PromptItem prompt)
        {
            switch (NormalizePromptTitle(prompt.Title))
            {
                case "generate sql query from description":
                    return 0;
                case "generate table from description":
                    return 1;
                case "explain selected query":
                    return 2;
                case "diagnose sql issue / wrong result":
                case "diagnose wrong / unexpected result":
                    return 3;
                case "convert sql dialect":
                    return 4;
                case "generate insert test data":
                    return 5;
                default:
                    return 1000;
            }
        }

        private static bool PromptMatchesSearch(PromptItem prompt, string rawSearchText)
        {
            var searchText = (rawSearchText ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(searchText))
                return true;

            var tokens = searchText
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => (t ?? string.Empty).Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToArray();

            if (tokens.Length == 0)
                return true;

            var tags = prompt.Tags ?? Array.Empty<string>();

            bool tagHas(string needle)
            {
                if (string.IsNullOrWhiteSpace(needle))
                    return true;
                return tags.Any(t => ContainsIgnoreCase(t, needle));
            }

            bool textHas(string needle)
            {
                return ContainsIgnoreCase(prompt.Title, needle)
                       || ContainsIgnoreCase(prompt.Description, needle)
                       || tags.Any(t => ContainsIgnoreCase(t, needle));
            }

            foreach (var token in tokens)
            {
                if (token.StartsWith("#") && token.Length > 1)
                {
                    if (!tagHas(token.Substring(1)))
                        return false;
                }
                else if (token.StartsWith("tag:", StringComparison.OrdinalIgnoreCase) && token.Length > 4)
                {
                    if (!tagHas(token.Substring(4)))
                        return false;
                }
                else
                {
                    if (!textHas(token))
                        return false;
                }
            }

            return true;
        }

        private List<PromptItem> GetFilteredPrompts(string rawSearchText)
        {
            IEnumerable<PromptItem> query = _prompts ?? new List<PromptItem>();

            query = query.Where(p => PromptMatchesSearch(p, rawSearchText));

            var selectedCategory = cmbPromptSource.SelectedItem as string;
            if (!string.IsNullOrWhiteSpace(selectedCategory)
                && !string.Equals(selectedCategory, "All categories", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(p => string.Equals(GetDisplayCategory(p), selectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            return query
                .OrderByDescending(IsFavoritePrompt)
                .ThenBy(GetDefaultPriorityRank)
                .ThenBy(p => (p.Title ?? string.Empty).Trim(), StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private void RefreshPromptList()
        {
            RefreshCategoryFilter();
            _filteredPrompts = GetFilteredPrompts(txtSearch.Text);
            PopulatePromptList();
        }

        private static Color BlendColor(Color baseColor, Color overlayColor, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));

            int r = (int)(baseColor.R + ((overlayColor.R - baseColor.R) * amount));
            int g = (int)(baseColor.G + ((overlayColor.G - baseColor.G) * amount));
            int b = (int)(baseColor.B + ((overlayColor.B - baseColor.B) * amount));

            return Color.FromArgb(baseColor.A, r, g, b);
        }

        private void ApplyRowStyling(DataGridViewRow row, PromptItem prompt)
        {
            var pal = UiThemeManager.Current;
            var isAlternateRow = (row.Index % 2) == 1;

            if (pal.IsDark)
            {
                row.DefaultCellStyle.BackColor = isAlternateRow
                    ? BlendColor(pal.PureBackground, Color.White, 0.05f)
                    : pal.PureBackground;
                row.DefaultCellStyle.SelectionBackColor = pal.HotBackground;
                row.DefaultCellStyle.SelectionForeColor = pal.Text;
                return;
            }

            row.DefaultCellStyle.BackColor = isAlternateRow
                ? BlendColor(SystemColors.Window, Color.Black, 0.035f)
                : SystemColors.Window;
            row.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
            row.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
        }

        private void PopulatePromptList()
        {
            var previouslySelectedPrompt = GetSelectedPrompt();

            _suppressPromptGridSelectionChanged = true;
            try
            {
                promptsGridView.Rows.Clear();

                foreach (var prompt in _filteredPrompts)
                {
                    var rowIndex = promptsGridView.Rows.Add(GetPromptDisplayTitle(prompt), prompt.Description, GetDisplayCategory(prompt));
                    var row = promptsGridView.Rows[rowIndex];
                    row.Tag = prompt;
                    row.DividerHeight = GetVisibleTags(prompt.Tags).Length > 0 ? ScaleUi(28) : ScaleUi(10);
                    ApplyRowStyling(row, prompt);
                }

                promptsGridView.ClearSelection();
                if (promptsGridView.Rows.Count > 0)
                    promptsGridView.CurrentCell = null;

                if (previouslySelectedPrompt.HasValue)
                {
                    foreach (DataGridViewRow row in promptsGridView.Rows)
                    {
                        if (!(row.Tag is PromptItem prompt) || prompt.Id != previouslySelectedPrompt.Value.Id)
                            continue;

                        row.Selected = true;
                        if (row.Cells.Count > 0)
                            promptsGridView.CurrentCell = row.Cells[0];

                        _showBlockingValidation = false;
                        GeneratePlaceholderControls(prompt);
                        UpdatePromptTextBoxForCurrentMode(prompt);
                        UpdatePromptMeta(prompt);
                        UpdateTagsBox(prompt);
                        UpdatePromptInfoFields(prompt);
                        UpdatePreviewTitle(prompt);
                        return;
                    }
                }

                UpdatePromptMeta(null);
                UpdateTagsBox(null);
                UpdatePromptInfoFields(null);
                UpdatePreviewTitle();

                if (promptsGridView.Rows.Count == 0)
                {
                    promptTextBox.Text = string.Empty;
                    flowLayoutPanelPlaceholders.Controls.Clear();
                    UpdateTagsBox(null);
                    UpdatePromptInfoFields(null);
                    UpdateCopyButtonState(false, "No prompts match the search criteria.");
                }
                else
                {
                    _showBlockingValidation = false;
                    SetPreviewText(string.Empty, false);
                    flowLayoutPanelPlaceholders.Controls.Clear();
                    UpdateCopyButtonState(false, "No prompt selected");
                }
            }
            finally
            {
                _suppressPromptGridSelectionChanged = false;
            }
        }

        private void promptsGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var row = promptsGridView.Rows[e.RowIndex];
            var pal = UiThemeManager.Current;
            var lineColor = pal.IsDark
                ? BlendColor(pal.Edge, Color.White, 0.10f)
                : BlendColor(SystemColors.ControlDark, Color.White, 0.25f);

            var startX = e.RowBounds.Left;
            if (promptsGridView.RowHeadersVisible)
                startX += promptsGridView.RowHeadersWidth;

            var contentWidth = e.RowBounds.Width - (startX - e.RowBounds.Left);
            var tags = row.Tag is PromptItem prompt ? GetVisibleTags(prompt.Tags) : Array.Empty<string>();

            if (row.DividerHeight > 0 && tags.Length > 0)
            {
                var rect = new Rectangle(
                    startX,
                    e.RowBounds.Bottom - row.DividerHeight,
                    Math.Max(0, contentWidth),
                    row.DividerHeight);

                var isSelected = row.Selected;
                var bgColor = isSelected ? row.DefaultCellStyle.SelectionBackColor : row.DefaultCellStyle.BackColor;

                using (var bgBrush = new SolidBrush(bgColor))
                {
                    e.Graphics.FillRectangle(bgBrush, rect);
                }

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (var font = new Font(promptsGridView.Font.FontFamily, Math.Max(1f, promptsGridView.Font.SizeInPoints + BasePromptGridTagFontDelta), FontStyle.Regular))
                {
                    var pillX = rect.Left + ScaleUi(BasePromptGridTagPaddingX);
                    var pillY = rect.Top + ScaleUi(BasePromptGridTagPaddingY);
                    var pillHeight = Math.Max(ScaleUi(BasePromptGridTagMinHeight), rect.Height - ScaleUi(BasePromptGridTagPaddingY * 2));
                    var availableRight = rect.Right - ScaleUi(BasePromptGridTagPaddingX);
                    var pillTextColor = isSelected ? row.DefaultCellStyle.SelectionForeColor : (pal.IsDark ? pal.Text : SystemColors.ControlText);
                    var pillFillColor = isSelected
                        ? BlendColor(bgColor, Color.White, pal.IsDark ? 0.18f : 0.30f)
                        : pal.IsDark
                            ? BlendColor(bgColor, Color.White, 0.12f)
                            : BlendColor(bgColor, SystemColors.Highlight, 0.10f);
                    var pillBorderColor = isSelected
                        ? BlendColor(bgColor, Color.White, pal.IsDark ? 0.28f : 0.40f)
                        : pal.IsDark
                            ? BlendColor(bgColor, Color.White, 0.22f)
                            : BlendColor(SystemColors.Highlight, Color.White, 0.18f);

                    foreach (var tag in tags)
                    {
                        var textSize = TextRenderer.MeasureText(e.Graphics, tag, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                        var pillWidth = Math.Max(ScaleUi(BasePromptGridTagMinWidth), textSize.Width + ScaleUi(BasePromptGridTagOverflowWidth));

                        if (pillX + pillWidth > availableRight)
                        {
                            var overflowWidth = TextRenderer.MeasureText(e.Graphics, "…", font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width + ScaleUi(BasePromptGridTagOverflowWidth);
                            if (pillX + overflowWidth <= availableRight)
                            {
                                var overflowRect = new Rectangle(pillX, pillY, overflowWidth, pillHeight);
                                using (var path = CreateRoundedRectangle(overflowRect, ScaleUi(8)))
                                using (var fillBrush = new SolidBrush(pillFillColor))
                                using (var borderPen = new Pen(pillBorderColor))
                                {
                                    e.Graphics.FillPath(fillBrush, path);
                                    e.Graphics.DrawPath(borderPen, path);
                                }

                                TextRenderer.DrawText(e.Graphics, "…", font, overflowRect, pillTextColor,
                                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                            }
                            break;
                        }

                        var pillRect = new Rectangle(pillX, pillY, pillWidth, pillHeight);
                        using (var path = CreateRoundedRectangle(pillRect, ScaleUi(8)))
                        using (var fillBrush = new SolidBrush(pillFillColor))
                        using (var borderPen = new Pen(pillBorderColor))
                        {
                            e.Graphics.FillPath(fillBrush, path);
                            e.Graphics.DrawPath(borderPen, path);
                        }

                        TextRenderer.DrawText(e.Graphics, tag, font, pillRect, pillTextColor,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                        pillX += pillWidth + ScaleUi(BasePromptGridTagGap);
                    }
                }

                e.Graphics.SmoothingMode = SmoothingMode.Default;
            }

            using (var linePen = new Pen(lineColor))
            {
                e.Graphics.DrawLine(linePen, startX + ScaleUi(BasePromptGridLineInset), e.RowBounds.Bottom - 1, startX + Math.Max(0, contentWidth - ScaleUi(BasePromptGridLineInset)), e.RowBounds.Bottom - 1);
            }
        }
        
        private void promptsGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            var hit = promptsGridView.HitTest(e.X, e.Y);
            if (hit.RowIndex < 0) return;

            var row = promptsGridView.Rows[hit.RowIndex];
            if (row.DividerHeight <= 0) return;

            var tags = row.Tag is PromptItem prompt ? GetVisibleTags(prompt.Tags) : Array.Empty<string>();
            if (tags.Length == 0) return;

            var rowBounds = promptsGridView.GetRowDisplayRectangle(hit.RowIndex, false);
            
            var startX = rowBounds.Left;
            if (promptsGridView.RowHeadersVisible) startX += promptsGridView.RowHeadersWidth;

            var contentWidth = rowBounds.Width - (startX - rowBounds.Left);
            var tagsRect = new Rectangle(startX, rowBounds.Bottom - row.DividerHeight, Math.Max(0, contentWidth), row.DividerHeight);

            if (!tagsRect.Contains(e.Location)) return;

            using (var font = new Font(promptsGridView.Font.FontFamily, Math.Max(1f, promptsGridView.Font.SizeInPoints + BasePromptGridTagFontDelta), FontStyle.Regular))
            {
                var pillX = tagsRect.Left + ScaleUi(BasePromptGridTagPaddingX);
                var pillY = tagsRect.Top + ScaleUi(BasePromptGridTagPaddingY);
                var pillHeight = Math.Max(ScaleUi(BasePromptGridTagMinHeight), tagsRect.Height - ScaleUi(BasePromptGridTagPaddingY) * 2);

                foreach (var tag in tags)
                {
                    var textRect = TextRenderer.MeasureText(tag, font);
                    var pillWidth = Math.Max(ScaleUi(BasePromptGridTagMinWidth), textRect.Width + ScaleUi(12));

                    if (pillX + pillWidth > tagsRect.Right - ScaleUi(BasePromptGridTagOverflowWidth))
                        break;

                    var pillRect = new Rectangle(pillX, pillY, pillWidth, pillHeight);
                    
                    if (pillRect.Contains(e.Location))
                    {
                        txtSearch.Text = $"tag:{tag}";
                        txtSearch.Focus();
                        txtSearch.SelectionStart = txtSearch.Text.Length;
                        return;
                    }

                    pillX += pillWidth + ScaleUi(BasePromptGridTagGap);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            buttonClearSearch.Visible = !string.IsNullOrEmpty(txtSearch.Text);
            if (!buttonClearSearch.Visible)
                _isSearchClearHover = false;
            buttonClearSearch.Invalidate();
            FlushPendingAutoSave();
            RefreshPromptList();
        }

        private void buttonClearSearch_Click(object sender, EventArgs e)
        {
            _isSearchClearHover = false;
            txtSearch.Clear();
            txtSearch.Focus();
        }

        private void buttonClearSearch_MouseEnter(object sender, EventArgs e)
        {
            _isSearchClearHover = true;
            buttonClearSearch.Invalidate();
        }

        private void buttonClearSearch_MouseLeave(object sender, EventArgs e)
        {
            _isSearchClearHover = false;
            buttonClearSearch.Invalidate();
        }

        private void buttonClearSearch_Paint(object sender, PaintEventArgs e)
        {
            var pal = UiThemeManager.Current;
            var backColor = _isSearchClearHover
                ? (pal.IsDark ? BlendColor(txtSearch.BackColor, Color.White, 0.12f) : BlendColor(txtSearch.BackColor, Color.Black, 0.08f))
                : txtSearch.BackColor;
            var lineColor = _isSearchClearHover
                ? (pal.IsDark ? Color.White : Color.Black)
                : (pal.IsDark ? pal.Text : Color.FromArgb(70, 70, 70));

            e.Graphics.Clear(backColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var pad = 2;
            using (var pen = new Pen(lineColor, 1.6f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                e.Graphics.DrawLine(pen, pad, pad, buttonClearSearch.Width - pad - 1, buttonClearSearch.Height - pad - 1);
                e.Graphics.DrawLine(pen, buttonClearSearch.Width - pad - 1, pad, pad, buttonClearSearch.Height - pad - 1);
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Escape || string.IsNullOrEmpty(txtSearch.Text))
                return;

            txtSearch.Clear();
            txtSearch.Focus();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void cmbPromptSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressCategoryFilterChanged)
                return;

            FlushPendingAutoSave();
            RefreshPromptList();
        }
        
        private void promptsGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (_isEditingTemplate) return;

            promptsGridView.ClearSelection();
            var row = promptsGridView.Rows[e.RowIndex];
            row.Selected = true;

            if (e.ColumnIndex >= 0 && e.ColumnIndex < row.Cells.Count)
                promptsGridView.CurrentCell = row.Cells[e.ColumnIndex];

            editingModeCheckbox.Checked = true;
            txtPromptName.Focus();
            txtPromptName.SelectAll();
        }
        
        private PromptItem? GetSelectedPrompt()
        {
            if (promptsGridView.SelectedRows.Count == 0)
                return null;

            return promptsGridView.SelectedRows[0].Tag is PromptItem prompt
                ? prompt
                : (PromptItem?)null;
        }

        private void UpdatePreviewTitle(PromptItem? prompt = null)
        {
            if (!prompt.HasValue)
                prompt = GetSelectedPrompt();

            var baseTitle = _isEditingTemplate ? "Edit Template (Auto-Save)" : "Prompt Details";
            var promptTitle = prompt.HasValue ? (prompt.Value.Title ?? string.Empty).Trim() : string.Empty;

            grpPreview.Text = string.IsNullOrWhiteSpace(promptTitle)
                ? baseTitle
                : $"{baseTitle} - {promptTitle}";
        }

        private void UpdatePromptMeta(PromptItem? prompt)
        {
            _actionToolTip.SetToolTip(buttonDuplicate, "Select a prompt to duplicate.");

            var databaseName = GetPlaceholderValue("database_name");
            var tableName = GetPlaceholderValue("table_name");

            if (!string.IsNullOrWhiteSpace(databaseName) && !string.IsNullOrWhiteSpace(tableName))
                lblPromptType.Text = $"Database: {databaseName}    Table: {tableName}";
            else if (!string.IsNullOrWhiteSpace(databaseName))
                lblPromptType.Text = $"Database: {databaseName}";
            else
                lblPromptType.Text = "No database selected";

            lblPromptCapabilities.Visible = false;
            lblPromptCapabilities.Text = string.Empty;

            _actionToolTip.SetToolTip(lblPromptType, lblPromptType.Text);
        }

        private void promptsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressPromptGridSelectionChanged)
                return;

            FlushPendingAutoSave();

            if (promptsGridView.SelectedRows.Count > 0)
            {
                var selectedRow = promptsGridView.SelectedRows[0];

                if (!(selectedRow.Tag is PromptItem prompt))
                    return;

                _showBlockingValidation = false;
                GeneratePlaceholderControls(prompt);
                UpdatePromptTextBoxForCurrentMode(prompt);
                UpdatePromptMeta(prompt);
                UpdateTagsBox(prompt);
                UpdatePromptInfoFields(prompt);
                UpdatePreviewTitle(prompt);
            }
            else
            {
                _showBlockingValidation = false;
                SetPreviewText(string.Empty, false);
                flowLayoutPanelPlaceholders.Controls.Clear();
                UpdateTagsBox(null);
                UpdatePromptInfoFields(null);
                UpdateCopyButtonState(false, "No prompt selected");
                UpdatePromptMeta(null);
                UpdatePreviewTitle();
            }
        }

        private void UpdateTagsBox(PromptItem? prompt)
        {
            _suppressTagsTextBoxChange = true;
            try
            {
                if (!prompt.HasValue)
                {
                    txtTags.Text = string.Empty;
                    return;
                }

                txtTags.Text = FormatTags(prompt.Value.Tags);
            }
            finally
            {
                _suppressTagsTextBoxChange = false;
            }
        }

        private void UpdatePromptInfoFields(PromptItem? prompt)
        {
            _suppressPromptMetaTextBoxChange = true;
            try
            {
                if (!prompt.HasValue)
                {
                    txtPromptName.Text = string.Empty;
                    txtPromptDescription.Text = string.Empty;
                    txtPromptCategory.Text = string.Empty;
                    return;
                }

                txtPromptName.Text = prompt.Value.Title ?? string.Empty;
                txtPromptDescription.Text = prompt.Value.Description ?? string.Empty;
                txtPromptCategory.Text = GetDisplayCategory(prompt.Value);
            }
            finally
            {
                _suppressPromptMetaTextBoxChange = false;
            }
        }

        private void SaveSelectedPromptMetadata(Func<PromptItem, PromptItem> updateAction)
        {
            if (_suppressPromptMetaTextBoxChange || !_isEditingTemplate)
                return;

            if (promptsGridView.SelectedRows.Count == 0)
                return;

            var selectedRow = promptsGridView.SelectedRows[0];
            if (!(selectedRow.Tag is PromptItem prompt))
                return;

            var beforeTitle = prompt.Title ?? string.Empty;
            var beforeDescription = prompt.Description ?? string.Empty;
            var beforeCategory = GetDisplayCategory(prompt);

            prompt = updateAction(prompt);

            var afterTitle = prompt.Title ?? string.Empty;
            var afterDescription = prompt.Description ?? string.Empty;
            var afterCategory = GetDisplayCategory(prompt);

            if (beforeTitle == afterTitle && beforeDescription == afterDescription && beforeCategory == afterCategory)
                return;

            selectedRow.Tag = prompt;
            selectedRow.Cells[colPromptName.Index].Value = prompt.Title;
            selectedRow.Cells[colPromptDesc.Index].Value = prompt.Description;
            selectedRow.Cells[colPromptType.Index].Value = afterCategory;
            ReplacePromptInCollections(prompt);
            UpdatePreviewTitle(prompt);

            _pendingAutoSavePrompt = prompt;
            _templateAutoSaveTimer.Stop();
            _templateAutoSaveTimer.Start();
        }

        private void txtPromptName_TextChanged(object sender, EventArgs e)
        {
            SaveSelectedPromptMetadata(prompt =>
            {
                var value = (txtPromptName.Text ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(value))
                    prompt.Title = value;
                return prompt;
            });
        }

        private void txtPromptDescription_TextChanged(object sender, EventArgs e)
        {
            SaveSelectedPromptMetadata(prompt =>
            {
                prompt.Description = (txtPromptDescription.Text ?? string.Empty).Trim();
                return prompt;
            });
        }

        private void txtPromptCategory_Leave(object sender, EventArgs e)
        {
            var selectedPromptId = promptsGridView.SelectedRows.Count > 0 && promptsGridView.SelectedRows[0].Tag is PromptItem selectedPrompt
                ? selectedPrompt.Id
                : null;

            SaveSelectedPromptMetadata(prompt =>
            {
                var value = (txtPromptCategory.Text ?? string.Empty).Trim();
                prompt.Category = string.IsNullOrWhiteSpace(value) ? "Custom" : value;
                return prompt;
            });

            RefreshCategoryFilter();
            RefreshPromptList();

            if (!string.IsNullOrWhiteSpace(selectedPromptId))
                SelectPromptById(selectedPromptId);
        }
        
        private bool HasMeaningfulPlaceholderValue(string placeholderName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var trimmed = value.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                return false;

            var hint = GetDefaultPlaceholderText(placeholderName);
            if (!string.IsNullOrEmpty(hint) && string.Equals(trimmed, hint, StringComparison.Ordinal))
                return false;

            return true;
        }

        private void GeneratePlaceholderControls(PromptItem prompt)
        {
            var pal = UiThemeManager.Current;

            flowLayoutPanelPlaceholders.Controls.Clear();
            flowLayoutPanelPlaceholders.SuspendLayout();

            _placeholdersInCurrentView.Clear();

            if (prompt.Placeholders != null)
            {
                foreach (var placeholder in prompt.Placeholders)
                {
                    if (string.IsNullOrEmpty(placeholder.Name) || _placeholdersInCurrentView.Contains(placeholder.Name))
                        continue;

                    _placeholdersInCurrentView.Add(placeholder.Name);

                    var container = new FlowLayoutPanel
                    {
                        FlowDirection = FlowDirection.TopDown,
                        AutoSize = true,
                        Width = flowLayoutPanelPlaceholders.Width - ScaleUi(25),
                        Margin = new Padding(0, 0, 0, ScaleUi(10)),
                        Tag = placeholder.Name
                    };

                    var headerRow = new FlowLayoutPanel
                    {
                        AutoSize = true,
                        WrapContents = false,
                        FlowDirection = FlowDirection.LeftToRight,
                        Margin = new Padding(0, 0, 0, ScaleUi(2)),
                        Name = "panelFieldHeader",
                        Width = container.Width
                    };

                    var label = new Label
                    {
                        AutoSize = true,
                        Margin = new Padding(0, 0, ScaleUi(8), 0),
                        Padding = new Padding(0, ScaleUi(2), 0, ScaleUi(2)),
                        Name = "lblFieldTitle",
                        Text = GetPlaceholderDisplayName(placeholder.Name),
                        Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold),
                        ForeColor = pal.IsDark ? pal.Text : Color.Black,
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    _actionToolTip.SetToolTip(label, "This prompt input is required.");

                    var modeBadge = CreatePlaceholderBadge("lblFieldModeBadge", string.Empty);
                    var valueBadge = CreatePlaceholderBadge("lblFieldValueBadge", string.Empty);

                    _actionToolTip.SetToolTip(modeBadge, GetAutoFillTooltip(placeholder.Name));
                    _actionToolTip.SetToolTip(valueBadge, "This input is required but it is not filled in yet.");

                    headerRow.Controls.Add(label);
                    headerRow.Controls.Add(modeBadge);
                    headerRow.Controls.Add(valueBadge);
                    container.Controls.Add(headerRow);

                    var initialValue = string.Empty;
                    var hasInitialValue = Placeholders.TryGetValue(placeholder.Name, out initialValue)
                                          && HasMeaningfulPlaceholderValue(placeholder.Name, initialValue);

                    var statusLabel = new Label
                    {
                        AutoSize = true,
                        Margin = new Padding(0, 0, 0, ScaleUi(6)),
                        Name = "lblFieldStatus",
                        ForeColor = GetPlaceholderHintForeColor(),
                        Text = GetPlaceholderStatusText(placeholder.Name, hasInitialValue)
                    };
                    container.Controls.Add(statusLabel);

                    var inputHost = new Panel
                    {
                        Width = container.Width,
                        Height = ScaleUi(BasePlaceholderInputHeight),
                        Margin = new Padding(0),
                        Name = "pnlInputHost",
                        BackColor = pal.IsDark ? pal.PureBackground : SystemColors.Window
                    };

                    var stateAccent = new Panel
                    {
                        Width = ScaleUi(BasePromptGridAccentWidth),
                        Margin = new Padding(0),
                        Name = "pnlFieldIndicator",
                        BackColor = pal.IsDark ? pal.Edge : SystemColors.ControlDark,
                        Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left,
                        Location = new Point(0, 0),
                        Height = inputHost.Height
                    };
                    inputHost.Controls.Add(stateAccent);

                    var inputControl = new RichTextBox
                    {
                        Tag = placeholder.Name,
                        BorderStyle = BorderStyle.FixedSingle,
                        ScrollBars = RichTextBoxScrollBars.Vertical,
                        Margin = new Padding(0),
                        Name = "rtbPlaceholderValue",
                        Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                        Location = new Point(stateAccent.Width + ScaleUi(BasePromptGridInputGap), 0),
                        Width = Math.Max(ScaleUi(BasePromptGridMinInputWidth), inputHost.Width - stateAccent.Width - ScaleUi(BasePromptGridInputGap)),
                        Height = inputHost.Height
                    };

                    if (hasInitialValue)
                    {
                        SetPlaceholderAsValue(inputControl, initialValue);
                    }
                    else
                    {
                        var defaultText = GetDefaultPlaceholderText(placeholder.Name);
                        if (!string.IsNullOrEmpty(defaultText))
                            SetPlaceholderAsHint(inputControl);
                        else
                            SetPlaceholderAsValue(inputControl, string.Empty);

                        Placeholders[placeholder.Name] = string.Empty;
                    }

                    inputControl.TextChanged += InputControl_TextChanged;
                    inputControl.Enter += InputControl_Enter;
                    inputControl.Leave += InputControl_Leave;
                    inputHost.Controls.Add(inputControl);
                    inputHost.Resize += PlaceholderInputHost_Resize;
                    LayoutPlaceholderInputHost(inputHost);
                    container.Controls.Add(inputHost);
                    UpdatePlaceholderBadges(container, hasInitialValue, false);

                    var grip = new Panel
                    {
                        Height = ScaleUi(BasePlaceholderGripHeight),
                        Dock = DockStyle.Bottom,
                        Cursor = Cursors.SizeNS,
                        BackColor = pal.IsDark ? pal.Edge : SystemColors.ControlDark,
                        Tag = inputHost
                    };
                    grip.MouseDown += Grip_MouseDown;
                    grip.MouseMove += Grip_MouseMove;
                    grip.MouseUp += Grip_MouseUp;
                    container.Controls.Add(grip);

                    flowLayoutPanelPlaceholders.Controls.Add(container);
                }
            }

            UiThemeManager.Apply(flowLayoutPanelPlaceholders);

            flowLayoutPanelPlaceholders.ResumeLayout();

            ResizePlaceholderControlWidths();

            ValidateInputs();
        }

        private void grpPreview_Resize(object sender, EventArgs e)
        {
            if (_previewCollapsed)
            {
                panelPreviewBottom.Height = GetCollapsedPlaceholdersHeight();
                return;
            }

            ApplyPreviewBottomHeight(panelPreviewBottom.Height);
            _lastExpandedPlaceholdersHeight = panelPreviewBottom.Height;
        }

        private void splitterPreview_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _previewCollapsed)
                return;

            ApplyPreviewBottomHeight(GetDefaultPlaceholdersHeight());
            _lastExpandedPlaceholdersHeight = panelPreviewBottom.Height;
        }

        private void splitterPreview_Paint(object sender, PaintEventArgs e)
        {
            ApplyPreviewSplitterVisuals();

            var pal = UiThemeManager.Current;
            var lineColor = pal.IsDark ? pal.Edge : SystemColors.ControlDark;
            var centerX = splitterPreview.ClientSize.Width / 2;
            var centerY = splitterPreview.ClientSize.Height / 2;
            var gripWidth = ScaleUi(BaseSplitterGripWidth);

            using (var pen = new Pen(lineColor))
            {
                for (int i = -1; i <= 1; i++)
                {
                    var y = centerY + (i * 3);
                    e.Graphics.DrawLine(pen, centerX - (gripWidth / 2), y, centerX + (gripWidth / 2), y);
                }
            }
        }

        private void Grip_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _resizingControl = (Control)((Panel)sender).Tag;
                _resizeStartY = Cursor.Position.Y;
                ((Panel)sender).Capture = true;
            }
        }

        private void Grip_MouseMove(object sender, MouseEventArgs e)
        {
            if (_resizingControl != null && e.Button == MouseButtons.Left)
            {
                var delta = Cursor.Position.Y - _resizeStartY;
                _resizingControl.Height = Math.Max(ScaleUi(BasePlaceholderInputHeight), _resizingControl.Height + delta);
                _resizeStartY = Cursor.Position.Y;

                if (_resizingControl.Controls.OfType<RichTextBox>().FirstOrDefault() is RichTextBox input)
                    input.Height = _resizingControl.ClientSize.Height;

                _resizingControl.PerformLayout();
                flowLayoutPanelPlaceholders.PerformLayout();
            }
        }

        private void Grip_MouseUp(object sender, MouseEventArgs e)
        {
            _resizingControl = null;
            if (sender is Panel p) p.Capture = false;
        }

        private void InputControl_TextChanged(object sender, EventArgs e)
        {
            var control = sender as RichTextBox;
            if (control == null)
                return;

            var key = control.Tag as string;
            if (string.IsNullOrEmpty(key))
                return;

            Placeholders[key] = IsPlaceholderHint(control) ? string.Empty : (control.Text ?? string.Empty).Trim();

            if (promptsGridView.SelectedRows.Count > 0)
            {
                var prompt = (PromptItem)promptsGridView.SelectedRows[0].Tag;
                if (!_isEditingTemplate)
                    UpdatePreviewText(prompt);
            }

            ValidateInputs();
        }
        
        private string BuildPreviewDisplayText(string text)
        {
            var builder = new StringBuilder();
            var normalized = (text ?? string.Empty)
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");
            var lines = normalized.Split('\n');
            var inCodeBlock = false;
            var previousWasBlank = true;

            foreach (var rawLine in lines)
            {
                var line = rawLine ?? string.Empty;
                var trimmed = line.Trim();

                if (trimmed.StartsWith("```", StringComparison.Ordinal))
                {
                    inCodeBlock = !inCodeBlock;
                    continue;
                }

                var isHeading = !inCodeBlock
                                && trimmed.StartsWith("**", StringComparison.Ordinal)
                                && trimmed.EndsWith("**", StringComparison.Ordinal)
                                && trimmed.Length > 4;

                var displayLine = isHeading
                    ? trimmed.Substring(2, trimmed.Length - 4).Trim()
                    : line;

                if (isHeading && !previousWasBlank && builder.Length > 0)
                    builder.AppendLine();

                builder.AppendLine(displayLine);
                previousWasBlank = string.IsNullOrWhiteSpace(displayLine);
            }

            return builder.ToString().TrimEnd('\r', '\n');

        }

        private void SetPreviewText(string text, bool formatAsPreview)
        {
            promptTextBox.SuspendLayout();

            _suppressPromptTextBoxChange = true;
            try
            {
                promptTextBox.Clear();

                if (formatAsPreview)
                {
                    promptTextBox.Text = BuildPreviewDisplayText(text);
                    promptTextBox.SelectAll();
                    promptTextBox.SelectionFont = promptTextBox.Font;
                    promptTextBox.SelectionColor = UiThemeManager.Current.IsDark
                        ? UiThemeManager.Current.Text
                        : SystemColors.ControlText;
                }
                else
                {
                    promptTextBox.Text = text;
                    promptTextBox.SelectAll();
                    promptTextBox.SelectionFont = promptTextBox.Font;
                    promptTextBox.SelectionColor = promptTextBox.ForeColor;
                }

                promptTextBox.SelectionStart = 0;
                promptTextBox.SelectionLength = 0;
                promptTextBox.ScrollToCaret();
            }
            finally
            {
                _suppressPromptTextBoxChange = false;
                promptTextBox.ResumeLayout();
            }
        }

        private void UpdatePreviewText(PromptItem prompt)
        {
            if (_isEditingTemplate)
            {
                SetPreviewText(prompt.Text ?? string.Empty, false);
                return;
            }

            var baseText = ConstructPromptPreview(SubstitutePlaceholders(prompt.Text));
            SetPreviewText(baseText, true);
        }
        
        private void SetEditingMode(bool isEditing)
        {
            FlushPendingAutoSave();

            if (isEditing && _previewCollapsed)
            {
                _restoreCollapsedAfterEditing = true;
                SetPreviewCollapsed(false, false);
            }
            else if (!isEditing && _restoreCollapsedAfterEditing)
            {
                SetPreviewCollapsed(true, false);
                _restoreCollapsedAfterEditing = false;
            }

            _isEditingTemplate = isEditing;

            UpdatePreviewTitle();
            promptTextBox.BorderStyle = BorderStyle.FixedSingle;
            txtTags.BorderStyle = BorderStyle.FixedSingle;
            
            panelEditFields.Visible = isEditing;
            promptTextBox.ReadOnly = !isEditing;
            txtTags.ReadOnly = !isEditing;
            txtPromptName.ReadOnly = !isEditing;
            txtPromptDescription.ReadOnly = !isEditing;
            txtPromptCategory.ReadOnly = !isEditing;
            promptsGridView.ReadOnly = true;
            buttonTogglePreview.Enabled = !isEditing;
            promptTextBox.TabStop = isEditing;
            txtTags.TabStop = isEditing;
            txtPromptName.TabStop = isEditing;
            txtPromptDescription.TabStop = isEditing;
            txtPromptCategory.TabStop = isEditing;
            
            promptTextBox.Cursor = isEditing ? Cursors.IBeam : Cursors.Default;
            txtTags.Cursor = isEditing ? Cursors.IBeam : Cursors.Default;
            txtPromptName.Cursor = isEditing ? Cursors.IBeam : Cursors.Default;
            txtPromptDescription.Cursor = isEditing ? Cursors.IBeam : Cursors.Default;
            txtPromptCategory.Cursor = isEditing ? Cursors.IBeam : Cursors.Default;

            editingModeCheckbox.Text = _isEditingTemplate ? "Done" : "Edit";
            lblEditingBadge.Text = _isEditingTemplate ? "Edit mode" : "Preview";
            lblEditingBadge.ForeColor = Color.White;
            ApplyScaledLayoutMetrics();

            colPromptType.ReadOnly = true;
            colPromptName.ReadOnly = true;
            colPromptDesc.ReadOnly = true;

            var pal = UiThemeManager.Current;

            if (pal.IsDark)
            {
                promptTextBox.BackColor = isEditing ? pal.HotBackground : pal.SofterBackground;
                txtTags.BackColor = isEditing ? pal.HotBackground : pal.SofterBackground;
                txtPromptName.BackColor = isEditing ? pal.HotBackground : pal.SofterBackground;
                txtPromptDescription.BackColor = isEditing ? pal.HotBackground : pal.SofterBackground;
                txtPromptCategory.BackColor = isEditing ? pal.HotBackground : pal.SofterBackground;
            }
            else
            {
                promptTextBox.BackColor = isEditing ? Color.White : SystemColors.ControlLight;
                txtTags.BackColor = isEditing ? Color.White : SystemColors.ControlLight;
                txtPromptName.BackColor = isEditing ? Color.White : SystemColors.ControlLight;
                txtPromptDescription.BackColor = isEditing ? Color.White : SystemColors.ControlLight;
                txtPromptCategory.BackColor = isEditing ? Color.White : SystemColors.ControlLight;
            }

            _actionToolTip.SetToolTip(buttonTogglePreview,
                isEditing
                    ? "Preview stays visible while you are editing the template."
                    : (_previewCollapsed ? "Show the prompt preview again." : "Collapse the preview to focus on prompt inputs."));

            _actionToolTip.SetToolTip(buttonInsertPlaceholder,
                isEditing
                    ? "Insert a {{placeholder_name}} token at the caret."
                    : "Click to switch to Edit and insert a placeholder into the template.");

            if (promptsGridView.SelectedRows.Count > 0)
            {
                var prompt = (PromptItem)promptsGridView.SelectedRows[0].Tag;
                UpdatePromptTextBoxForCurrentMode(prompt);
                UpdatePromptInfoFields(prompt);
            }
        }
        
        private void promptsGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (!_isEditingTemplate) return;
            if (e.RowIndex < 0) return;

            var row = promptsGridView.Rows[e.RowIndex];
            if (!(row.Tag is PromptItem prompt)) return;

            bool changed = false;

            if (e.ColumnIndex == colPromptName.Index)
            {
                var v = Convert.ToString(row.Cells[colPromptName.Index].Value) ?? "";
                v = v.Trim();
                if (!string.IsNullOrWhiteSpace(v) && v != prompt.Title)
                {
                    prompt.Title = v;
                    changed = true;
                }
                else
                {
                    row.Cells[colPromptName.Index].Value = prompt.Title;
                }
            }
            else if (e.ColumnIndex == colPromptDesc.Index)
            {
                var v = Convert.ToString(row.Cells[colPromptDesc.Index].Value) ?? "";
                v = v.Trim();
                if (v != (prompt.Description ?? ""))
                {
                    prompt.Description = v;
                    changed = true;
                }
            }

            if (!changed) return;

            row.Tag = prompt;
            ReplacePromptInCollections(prompt);

            _pendingAutoSavePrompt = prompt;
            _templateAutoSaveTimer.Stop();
            _templateAutoSaveTimer.Start();
        }
        
        private void promptsGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewTextBoxEditingControl tb)
            {
                var pal = UiThemeManager.Current;

                tb.BorderStyle = BorderStyle.FixedSingle;
                tb.BackColor = pal.IsDark ? pal.HotBackground : Color.White;
                tb.ForeColor = pal.IsDark ? pal.Text : SystemColors.ControlText;
            }
        }

        private void txtTags_TextChanged(object sender, EventArgs e)
        {
            if (_suppressTagsTextBoxChange)
                return;

            if (!_isEditingTemplate)
                return;

            if (promptsGridView.SelectedRows.Count == 0)
                return;

            var selectedRow = promptsGridView.SelectedRows[0];
            if (!(selectedRow.Tag is PromptItem prompt))
                return;

            prompt.Tags = ParseTags(txtTags.Text);

            selectedRow.Tag = prompt;
            ReplacePromptInCollections(prompt);

            _pendingAutoSavePrompt = prompt;
            _templateAutoSaveTimer.Stop();
            _templateAutoSaveTimer.Start();
        }

        private void UpdatePromptTextBoxForCurrentMode(PromptItem prompt)
        {
            if (_isEditingTemplate)
                SetPreviewText(prompt.Text ?? string.Empty, false);
            else
                UpdatePreviewText(prompt);
        }

        private void promptTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_suppressPromptTextBoxChange)
                return;

            if (!_isEditingTemplate)
                return;

            if (promptsGridView.SelectedRows.Count == 0)
                return;

            var selectedRow = promptsGridView.SelectedRows[0];
            if (!(selectedRow.Tag is PromptItem prompt))
                return;

            prompt.Text = promptTextBox.Text;
            prompt.Placeholders = BuildPlaceholdersFromText(prompt.Text, prompt.Placeholders);

            selectedRow.Tag = prompt;
            ReplacePromptInCollections(prompt);

            _pendingAutoSavePrompt = prompt;
            _templateAutoSaveTimer.Stop();
            _templateAutoSaveTimer.Start();
        }

        private void TemplateAutoSaveTimer_Tick(object sender, EventArgs e)
        {
            _templateAutoSaveTimer.Stop();

            if (!_pendingAutoSavePrompt.HasValue)
                return;

            var promptToSave = _pendingAutoSavePrompt.Value;
            _pendingAutoSavePrompt = null;

            try
            {
                FrmPromptEditor.SaveNewPromptToFile(promptToSave);
                _autoSaveErrorShown = false;

                if (promptsGridView.SelectedRows.Count > 0)
                {
                    var selectedPrompt = (PromptItem)promptsGridView.SelectedRows[0].Tag;
                    if (selectedPrompt.Id == promptToSave.Id)
                    {
                        GeneratePlaceholderControls(promptToSave);
                        ValidateInputs();
                    }
                }
            }
            catch (Exception ex)
            {
                if (!_autoSaveErrorShown)
                {
                    _autoSaveErrorShown = true;
                    MessageBox.Show("Auto-save failed: " + ex.Message, "Prompt Auto-Save",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void FlushPendingAutoSave()
        {
            if (_templateAutoSaveTimer != null && _templateAutoSaveTimer.Enabled)
            {
                TemplateAutoSaveTimer_Tick(this, EventArgs.Empty);
            }
        }

        private void ReplacePromptInCollections(PromptItem updatedPrompt)
        {
            if (_prompts != null)
            {
                var index = _prompts.FindIndex(p => p.Id == updatedPrompt.Id);
                if (index >= 0)
                    _prompts[index] = updatedPrompt;
            }

            if (_filteredPrompts != null)
            {
                var index = _filteredPrompts.FindIndex(p => p.Id == updatedPrompt.Id);
                if (index >= 0)
                    _filteredPrompts[index] = updatedPrompt;
            }
        }

        private void SelectPromptById(string promptId)
        {
            if (string.IsNullOrWhiteSpace(promptId))
                return;

            for (int i = 0; i < promptsGridView.Rows.Count; i++)
            {
                var row = promptsGridView.Rows[i];
                if (!(row.Tag is PromptItem prompt) || !string.Equals(prompt.Id, promptId, StringComparison.OrdinalIgnoreCase))
                    continue;

                _suppressPromptGridSelectionChanged = true;
                try
                {
                    promptsGridView.ClearSelection();
                    row.Selected = true;
                    promptsGridView.CurrentCell = row.Cells[Math.Max(0, colPromptName.Index)];
                }
                finally
                {
                    _suppressPromptGridSelectionChanged = false;
                }

                promptsListView_SelectedIndexChanged(promptsGridView, EventArgs.Empty);
                return;
            }
        }

        private PromptPlaceholder[] BuildPlaceholdersFromText(string promptText, PromptPlaceholder[] existingPlaceholders)
        {
            var extracted = new List<PromptPlaceholder>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            const string pattern = @"\{\{(.*?)\}\}";
            foreach (System.Text.RegularExpressions.Match match in
                     System.Text.RegularExpressions.Regex.Matches(promptText ?? string.Empty, pattern))
            {
                var name = match.Groups[1].Value.Trim();
                if (string.IsNullOrEmpty(name))
                    continue;

                if (!seen.Add(name))
                    continue;

                var placeholder = new PromptPlaceholder { Name = name };

                if (existingPlaceholders != null)
                {
                    for (int i = 0; i < existingPlaceholders.Length; i++)
                    {
                        if (!string.Equals(existingPlaceholders[i].Name, name, StringComparison.OrdinalIgnoreCase))
                            continue;
                        break;
                    }
                }

                extracted.Add(placeholder);
            }

            return extracted.ToArray();
        }

        private string GetDefaultPlaceholderText(string placeholderName)
        {
            if (string.Equals(placeholderName, "selected_sql", StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            if (string.Equals(placeholderName, "dialect", StringComparison.OrdinalIgnoreCase))
                return "Auto-fills from the active database connection, or enter the SQL dialect manually.";

            if (string.Equals(placeholderName, "table_name", StringComparison.OrdinalIgnoreCase))
                return "Select a table in the DB Connect Manager to auto-fill, or enter the table name manually.";

            if (string.Equals(placeholderName, "table", StringComparison.OrdinalIgnoreCase))
                return "Select a table in the DB Connect Manager to auto-fill, or enter the metadata manually.";

            if (placeholderName.IndexOf("query", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Paste or type the query here.";

            if (placeholderName.IndexOf("dialect", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Enter the target SQL dialect here.";

            if (placeholderName.IndexOf("description", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Describe the issue or request here.";

            return string.Empty;
        }
        
        private bool IsPlaceholderHint(RichTextBox inputControl)
        {
            var key = inputControl.Tag as string;
            if (string.IsNullOrEmpty(key))
                return false;

            var hint = GetDefaultPlaceholderText(key);
            if (string.IsNullOrEmpty(hint))
                return false;

            var hintColor = GetPlaceholderHintForeColor();

            return inputControl.Text == hint && inputControl.ForeColor == hintColor;
        }

        private void SetPlaceholderAsHint(RichTextBox inputControl)
        {
            var key = inputControl.Tag as string;
            var hint = GetDefaultPlaceholderText(key);

            if (string.IsNullOrEmpty(hint))
                return;

            inputControl.Text = hint;
            inputControl.ForeColor = GetPlaceholderHintForeColor();
        }

        private void SetPlaceholderAsValue(RichTextBox inputControl, string value)
        {
            inputControl.Text = value ?? string.Empty;
            inputControl.ForeColor = UiThemeManager.Current.IsDark ? UiThemeManager.Current.Text : SystemColors.WindowText;
        }

        private void InputControl_Enter(object sender, EventArgs e)
        {
            var inputControl = sender as RichTextBox;
            if (inputControl == null)
                return;

            if (IsPlaceholderHint(inputControl))
                SetPlaceholderAsValue(inputControl, string.Empty);
        }

        private void InputControl_Leave(object sender, EventArgs e)
        {
            var inputControl = sender as RichTextBox;
            if (inputControl == null)
                return;

            var key = inputControl.Tag as string;
            if (string.IsNullOrEmpty(key))
                return;

            if (string.IsNullOrWhiteSpace(inputControl.Text))
            {
                Placeholders[key] = string.Empty;

                if (!string.IsNullOrEmpty(GetDefaultPlaceholderText(key)))
                    SetPlaceholderAsHint(inputControl);
                else
                    SetPlaceholderAsValue(inputControl, string.Empty);
            }
        }

        private string GetPlaceholderStatusText(string placeholderName, bool hasInitialValue)
        {
            if (hasInitialValue)
                return "Ready!";

            if (string.Equals(placeholderName, "selected_sql", StringComparison.OrdinalIgnoreCase))
                return "Select SQL code in the Notepad++ editor to auto-fill, or type it manually.";

            if (string.Equals(placeholderName, "dialect", StringComparison.OrdinalIgnoreCase))
                return "Auto-fills from the active database connection, or type it manually.";

            if (string.Equals(placeholderName, "table_name", StringComparison.OrdinalIgnoreCase))
                return "Select a table in the DB Connect Manager to auto-fill, or type it manually.";

            if (string.Equals(placeholderName, "table", StringComparison.OrdinalIgnoreCase))
                return "Select a table in the DB Connect Manager to auto-fill, or enter the metadata manually.";

            if (placeholderName.IndexOf("query", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Paste or type the query here.";

            if (placeholderName.IndexOf("dialect", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Enter the target SQL dialect here.";

            if (placeholderName.IndexOf("description", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Describe the issue or request here.";

            if (placeholderName.EndsWith("_name", StringComparison.OrdinalIgnoreCase)
                || placeholderName.IndexOf("name", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Enter the name here.";

            return "Enter the value here.";
        }

        private string GetMissingPlaceholderMessage(PromptPlaceholder placeholder)
        {
            var displayName = GetPlaceholderDisplayName(placeholder.Name);
            return displayName + " is required. " + GetPlaceholderManualInstruction(placeholder.Name);
        }

        private void ValidateInputs()
        {
            _firstMissingPlaceholderName = null;

            if (promptsGridView.SelectedRows.Count == 0)
            {
                UpdatePlaceholderValidationState();
                UpdateCopyButtonState(false, "No prompt selected");
                return;
            }

            var prompt = (PromptItem)promptsGridView.SelectedRows[0].Tag;
            var isValid = true;
            var disabledReason = "Fill all required prompt inputs before continuing.";

            if (prompt.Placeholders != null)
            {
                foreach (var ph in prompt.Placeholders)
                {
                    if (Placeholders.TryGetValue(ph.Name, out var val) && HasMeaningfulPlaceholderValue(ph.Name, val))
                        continue;

                    isValid = false;
                    _firstMissingPlaceholderName = ph.Name;
                    disabledReason = GetMissingPlaceholderMessage(ph);
                    break;
                }
            }

            UpdatePlaceholderValidationState();
            UpdateCopyButtonState(isValid, disabledReason);
        }

        private void UpdatePlaceholderValidationState()
        {
            var pal = UiThemeManager.Current;
            var validBackColor = pal.IsDark ? pal.PureBackground : SystemColors.Window;
            var incompleteBackColor = pal.IsDark ? pal.PureBackground : SystemColors.Window;
            var invalidBackColor = pal.IsDark ? pal.PureBackground : Color.FromArgb(255, 244, 244);
            var validTitleColor = pal.IsDark ? pal.Text : Color.Black;
            var invalidTitleColor = pal.IsDark ? Color.FromArgb(255, 182, 182) : Color.Firebrick;
            var validStatusColor = GetPlaceholderHintForeColor();
            var invalidStatusColor = pal.IsDark ? Color.FromArgb(255, 214, 214) : Color.Firebrick;
            var validAccentColor = pal.IsDark ? Color.FromArgb(82, 163, 118) : Color.FromArgb(55, 122, 72);
            var incompleteAccentColor = pal.IsDark ? Color.FromArgb(88, 103, 120) : Color.FromArgb(150, 163, 177);
            var invalidAccentColor = pal.IsDark ? Color.FromArgb(255, 112, 112) : Color.Firebrick;

            foreach (Control control in flowLayoutPanelPlaceholders.Controls)
            {
                if (!(control is FlowLayoutPanel container))
                    continue;

                var placeholderName = container.Tag as string;
                var hasValue = !string.IsNullOrWhiteSpace(placeholderName)
                               && Placeholders.TryGetValue(placeholderName, out var value)
                               && HasMeaningfulPlaceholderValue(placeholderName, value);
                var showBlockingState = !hasValue && _showBlockingValidation;

                var titleLabel = container.Controls.Find("lblFieldTitle", false).OfType<Label>().FirstOrDefault();
                var statusLabel = container.Controls.Find("lblFieldStatus", false).OfType<Label>().FirstOrDefault();
                var inputControl = container.Controls.Find("rtbPlaceholderValue", true).OfType<RichTextBox>().FirstOrDefault();
                var accentPanel = container.Controls.Find("pnlFieldIndicator", true).OfType<Panel>().FirstOrDefault();

                if (titleLabel != null)
                    titleLabel.ForeColor = showBlockingState ? invalidTitleColor : validTitleColor;

                if (statusLabel != null)
                {
                    statusLabel.ForeColor = showBlockingState ? invalidStatusColor : validStatusColor;

                    if (showBlockingState && !string.IsNullOrWhiteSpace(placeholderName))
                        statusLabel.Text = GetMissingPlaceholderMessage(new PromptPlaceholder { Name = placeholderName });
                    else if (!string.IsNullOrWhiteSpace(placeholderName))
                        statusLabel.Text = GetPlaceholderStatusText(placeholderName, hasValue);
                }

                if (inputControl != null)
                    inputControl.BackColor = hasValue ? validBackColor : (showBlockingState ? invalidBackColor : incompleteBackColor);

                if (accentPanel != null)
                    accentPanel.BackColor = hasValue ? validAccentColor : (showBlockingState ? invalidAccentColor : incompleteAccentColor);

                UpdatePlaceholderBadges(container, hasValue, showBlockingState);
            }
        }

        private void FocusFirstMissingPlaceholder()
        {
            if (string.IsNullOrWhiteSpace(_firstMissingPlaceholderName))
                return;

            foreach (Control control in flowLayoutPanelPlaceholders.Controls)
            {
                if (!(control is FlowLayoutPanel container))
                    continue;

                if (!string.Equals(container.Tag as string, _firstMissingPlaceholderName, StringComparison.OrdinalIgnoreCase))
                    continue;

                var inputControl = container.Controls.Find("rtbPlaceholderValue", true).OfType<RichTextBox>().FirstOrDefault();
                if (inputControl == null)
                    return;

                flowLayoutPanelPlaceholders.ScrollControlIntoView(container);
                inputControl.Focus();

                if (!IsPlaceholderHint(inputControl))
                {
                    inputControl.SelectionStart = 0;
                    inputControl.SelectionLength = inputControl.TextLength;
                }

                return;
            }
        }

        private void UpdateCopyButtonState(bool enabled, string reason)
        {
            var pal = UiThemeManager.Current;

            _canCopy = enabled;
            _copyDisabledReason = reason ?? string.Empty;

            buttonCopy.Text = "Copy Prompt";
            buttonCopy.Enabled = true;
            buttonAiStudio.Enabled = true;

            if (enabled)
            {
                buttonCopy.BackColor = pal.IsDark ? pal.HotBackground : SystemColors.Highlight;
                buttonCopy.ForeColor = pal.IsDark ? pal.Text : SystemColors.HighlightText;
                buttonAiStudio.BackColor = pal.IsDark ? pal.HotBackground : SystemColors.Control;
                buttonAiStudio.ForeColor = pal.IsDark ? pal.Text : SystemColors.ControlText;
                _actionToolTip.SetToolTip(buttonCopy, "Copy prompt to clipboard");
                _actionToolTip.SetToolTip(buttonAiStudio, "Open the configured external LLM URL");
            }
            else
            {
                buttonCopy.BackColor = pal.IsDark ? pal.SofterBackground : Color.Gainsboro;
                buttonCopy.ForeColor = pal.IsDark ? pal.DarkerText : Color.DimGray;
                buttonAiStudio.BackColor = pal.IsDark ? pal.SofterBackground : Color.Gainsboro;
                buttonAiStudio.ForeColor = pal.IsDark ? pal.DarkerText : Color.DimGray;
                _actionToolTip.SetToolTip(buttonCopy, _copyDisabledReason);
                _actionToolTip.SetToolTip(buttonAiStudio, _copyDisabledReason);
            }
        }

        private string SubstitutePlaceholders(string text)
        {
            text = text ?? string.Empty;

            var selectedSql = Placeholders.TryGetValue("selected_sql", out var plh)
                ? plh
                : string.Empty;

            text = text.Replace("{{selected_sql}}", selectedSql);
            
            foreach (var placeholder in Placeholders)
            {
                if (!string.IsNullOrEmpty(placeholder.Value))
                {
                    text = text.Replace("{{" + placeholder.Key + "}}", placeholder.Value);
                }
            }
            
            return text;
        }
        
        private string ConstructPromptPreview(string promptBase)
        {
            var PromptStringBuilder = new StringBuilder();
            PromptStringBuilder.Append(promptBase);
            PromptStringBuilder.Append(LoadUserPromptPreferences());
            return PromptStringBuilder.ToString();
        }
        
        private void PlaceholderInputHost_Resize(object sender, EventArgs e)
        {
            if (sender is Panel inputHost)
                LayoutPlaceholderInputHost(inputHost);
        }

        private void LayoutPlaceholderInputHost(Panel inputHost)
        {
            if (inputHost == null)
                return;

            var accent = inputHost.Controls.Find("pnlFieldIndicator", false).OfType<Panel>().FirstOrDefault();
            var input = inputHost.Controls.Find("rtbPlaceholderValue", false).OfType<RichTextBox>().FirstOrDefault();
            if (accent == null || input == null)
                return;

            accent.Location = new Point(0, 0);
            accent.Height = inputHost.ClientSize.Height;

            var left = accent.Width + ScaleUi(BasePromptGridInputGap);
            input.Location = new Point(left, 0);
            input.Size = new Size(Math.Max(ScaleUi(BasePromptGridMinInputWidth), inputHost.ClientSize.Width - left), inputHost.ClientSize.Height);
        }

        private void flowLayoutPanelPlaceholders_SizeChanged(object sender, EventArgs e)
        {
            ResizePlaceholderControlWidths();
        }

        private void ResizePlaceholderControlWidths()
        {
            var targetWidth = flowLayoutPanelPlaceholders.ClientSize.Width - ScaleUi(10) - SystemInformation.VerticalScrollBarWidth;
            if (targetWidth < ScaleUi(BasePromptGridMinContainerWidth)) targetWidth = ScaleUi(BasePromptGridMinContainerWidth);

            foreach (Control c in flowLayoutPanelPlaceholders.Controls)
            {
                if (!(c is FlowLayoutPanel container))
                    continue;

                container.Width = targetWidth;

                var headerRow = container.Controls.Find("panelFieldHeader", false).OfType<FlowLayoutPanel>().FirstOrDefault();
                if (headerRow != null)
                    headerRow.Width = targetWidth;

                var inputHost = container.Controls.Find("pnlInputHost", false).OfType<Panel>().FirstOrDefault();
                if (inputHost != null)
                {
                    inputHost.Width = targetWidth;
                    LayoutPlaceholderInputHost(inputHost);
                }

                var grip = container.Controls.OfType<Panel>().FirstOrDefault(p => p.Tag == inputHost);
                if (grip != null)
                    grip.Width = targetWidth;
            }
        }

        private string LoadUserPromptPreferences()
        {
            var preferences = NppDbSettingsStore.Get().Prompt;
            return $"\nRespond in: {preferences.ResponseLanguage}." +
                   $"\nCustom instructions: {preferences.CustomInstructions}.";
        }

        private void editingModeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            SetEditingMode(editingModeCheckbox.Checked);
        }

        private void buttonTogglePreview_Click(object sender, EventArgs e)
        {
            if (_isEditingTemplate)
                return;

            SetPreviewCollapsed(!_previewCollapsed, true);
        }

        private void buttonInsertPlaceholder_Click(object sender, EventArgs e)
        {
            if (promptsGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a prompt first.", "Add Prompt Input", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_isEditingTemplate)
            {
                editingModeCheckbox.Checked = true;
            }

            if (!_isEditingTemplate)
                return;

            var placeholderName = FrmPromptEditor.AskForPlaceholderName(this);
            if (string.IsNullOrWhiteSpace(placeholderName))
                return;

            FrmPromptEditor.InsertPlaceholderAtCaret(promptTextBox, placeholderName);
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FlushPendingAutoSave();
            using (var promptEditor = new FrmPromptEditor())
            {
                promptEditor.LoadPlaceholders(SupportedPlaceholders);

                var result = promptEditor.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var newPrompt = promptEditor.SelectedPromptItem;
                    _prompts.Add(newPrompt);
                    
                    // refresh search results to possibly include the new prompt
                    RefreshPromptList();
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            FlushPendingAutoSave();
            if (promptsGridView.SelectedRows.Count > 0)
            {
                var selectedRow = promptsGridView.SelectedRows[0];
                var prompt = (PromptItem)selectedRow.Tag;

                var confirmResult = MessageBox.Show(
                    $"Are you sure you want to delete the prompt '{prompt.Title}'?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    _prompts.RemoveAll(p => p.Id == prompt.Id);
                    
                    // Update UI via search logic
                    RefreshPromptList();

                    ErasePromptFromFile(prompt);

                    promptTextBox.Text = string.Empty;
                    flowLayoutPanelPlaceholders.Controls.Clear();
                    UpdateCopyButtonState(false, "No prompt selected");
                }
            }
        }

        private void ErasePromptFromFile(PromptItem promptItem)
        {
            string promptLibraryPath = PromptLibraryPath;
            if (!File.Exists(promptLibraryPath))
            {
                throw new FileNotFoundException("Prompt library file not found.", promptLibraryPath);
            }

            var doc = XDocument.Load(promptLibraryPath);
            var root = doc.Element("Prompts");
            if (root == null) return;

            var promptElement = Enumerable.FirstOrDefault(root.Elements("Prompt"), e =>
                (string)e.Element("Id") == promptItem.Id);
            if (promptElement != null)
            {
                promptElement.Remove();
                doc.Save(promptLibraryPath);
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (!_canCopy)
            {
                _showBlockingValidation = true;
                ValidateInputs();
                FocusFirstMissingPlaceholder();

                if (!string.IsNullOrWhiteSpace(_copyDisabledReason))
                    _actionToolTip.Show(_copyDisabledReason, buttonCopy, buttonCopy.Width / 2, -18, 2400);
                return;
            }

            var pal = UiThemeManager.Current;

            var textToCopy = promptTextBox.Text;

            if (promptsGridView.SelectedRows.Count > 0)
            {
                var prompt = (PromptItem)promptsGridView.SelectedRows[0].Tag;
                textToCopy = ConstructPromptPreview(SubstitutePlaceholders(prompt.Text));
            }

            if (!string.IsNullOrEmpty(textToCopy))
            {
                Clipboard.SetText(textToCopy);
                buttonCopy.BackColor = pal.IsDark ? pal.SofterBackground : Color.DarkBlue;
                buttonCopy.ForeColor = pal.IsDark ? pal.Text : buttonCopy.ForeColor;
                buttonCopy.Text = "Copied!";
                var timer = new Timer { Interval = 1000 };
                timer.Tick += (s, args) =>
                {
                    ValidateInputs(); 
                    timer.Stop();
                };
                timer.Start();
            }
        }

        private void buttonDuplicate_Click(object sender, EventArgs e)
        {
            FlushPendingAutoSave();
            if (promptsGridView.SelectedRows.Count == 0)
                return;

            var selectedRow = promptsGridView.SelectedRows[0];
            var prompt = (PromptItem)selectedRow.Tag;

            var newPrompt = prompt;
            newPrompt.Id = Guid.NewGuid().ToString();
            newPrompt.Title += " (Copy)";

            _prompts.Add(newPrompt);
            FrmPromptEditor.SaveNewPromptToFile(newPrompt);

            RefreshPromptList();
            SelectPromptById(newPrompt.Id);
            editingModeCheckbox.Checked = true;

            if (promptsGridView.SelectedRows.Count > 0)
            {
                txtPromptName.Focus();
                txtPromptName.SelectAll();
            }
        }
        
        private void buttonAiStudio_Click(object sender, EventArgs e)
        {
            if (!_canCopy)
            {
                _showBlockingValidation = true;
                ValidateInputs();
                FocusFirstMissingPlaceholder();

                if (!string.IsNullOrWhiteSpace(_copyDisabledReason))
                    _actionToolTip.Show(_copyDisabledReason, buttonAiStudio, buttonAiStudio.Width / 2, -18, 2400);
                return;
            }

            try
            {
                var url = NppDbSettingsStore.Get().Prompt.OpenLlmUrl?.Trim() ?? "https://chatgpt.com/";
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }
                
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open the browser: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void lblEditingBadge_forceWhiteText(object sender, EventArgs e)
        {
            if (lblEditingBadge != null && lblEditingBadge.ForeColor != Color.White)
            {
                lblEditingBadge.ForeColor = Color.White;
            }
        }
    }
}
