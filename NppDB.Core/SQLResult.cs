using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NppDB.Comm;
using NppDB.Core.Properties;
using NppDB.PostgreSQL;

namespace NppDB.Core
{
    public partial class SqlResult : UserControl
    {
        private static readonly List<WeakReference<SqlResult>> ActiveInstances = new List<WeakReference<SqlResult>>();
        private static readonly object ListLock = new object();
        
        public event Action<SqlResult, int> UserResizeRequested;
        private Panel _resizeGrip;
        private bool _isResizing;
        private int _resizeStartScreenY;
        private int _resizeStartHeight;

        public SqlResult(IDbConnect connect, ISqlExecutor sqlExecutor)
        {
            InitializeComponent();
            UiThemeManager.Register(this);
            Init();
            SetConnect(connect, sqlExecutor);

            lock (ListLock)
            {
                ActiveInstances.RemoveAll(wr => !wr.TryGetTarget(out _));
                ActiveInstances.Add(new WeakReference<SqlResult>(this));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (ListLock)
                {
                    ActiveInstances.RemoveAll(wr => !wr.TryGetTarget(out var target) || target == this);
                }
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private static void CloseTabsInAllActiveInstances()
        {
            var liveInstances = new List<SqlResult>();
            lock(ListLock)
            {
                ActiveInstances.RemoveAll(wr =>
                {
                    if (!wr.TryGetTarget(out var target)) {
                        return true;
                    }
                    liveInstances.Add(target);
                    return false;
                });
            }

            if (liveInstances.Count == 0)
            {
                return;
            }

            foreach (var instance in liveInstances)
            {
                try
                {
                    instance.CloseAllResultTabs();
                }
                catch (Exception ex)
                {
                     MessageBox.Show($@"Error closing tabs: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void CloseAllResultTabs()
        {
            if (tclSqlResult == null || tclSqlResult.TabPages.Count <= 1) return;
            try
            {
                if (tclSqlResult.SelectedIndex != 0)
                {
                    tclSqlResult.SelectTab(0);
                }

                for (var i = tclSqlResult.TabPages.Count - 1; i > 0; i--)
                {
                    CloseResultTab(i, false);
                }

                _selectedTabIndex = tclSqlResult.SelectedIndex;
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"An error occurred while closing result tabs in this view: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseOtherResultTabs(int tabIndex)
        {
            if (tclSqlResult == null || tabIndex <= 0 || tabIndex >= tclSqlResult.TabPages.Count) return;

            try
            {
                var selectedTab = tclSqlResult.SelectedTab;
                var tabToKeep = tclSqlResult.TabPages[tabIndex];

                for (var i = tclSqlResult.TabPages.Count - 1; i > 0; i--)
                {
                    if (tclSqlResult.TabPages[i] == tabToKeep) continue;
                    CloseResultTab(i, false);
                }

                if (selectedTab != null && tclSqlResult.TabPages.Contains(selectedTab))
                {
                    tclSqlResult.SelectTab(selectedTab);
                }
                else
                {
                    tclSqlResult.SelectTab(tabToKeep);
                }

                _selectedTabIndex = tclSqlResult.SelectedIndex;
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"An error occurred while closing other result tabs in this view: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int _selectedTabIndex;
        private int _tabCounter = 1;

        private void Init()
        {
            btnStop.Click += (s, e) =>
            {
                btnStop.Enabled = false;
                try
                {
                    _exec.Stop();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    btnStop.Enabled = _exec.CanStop();
                }
            };

            tclSqlResult.BackColor = UiThemeManager.Current.Background;
            tclSqlResult.DrawMode = TabDrawMode.OwnerDrawFixed;
            tclSqlResult.SizeMode = TabSizeMode.Fixed;

            tclSqlResult.Paint += (s, e) => // tab strip area
            {
                var pal = UiThemeManager.Current;
                var stripHeight = tclSqlResult.DisplayRectangle.Y;
                if (stripHeight <= 0) return;

                using (var b = new SolidBrush(pal.Background))
                {
                    e.Graphics.FillRectangle(b, new Rectangle(0, 0, tclSqlResult.Width, stripHeight));
                }
            };

            int initialWidth;
            int initialHeight;
            using (var g = CreateGraphics())
            {
                 var tabFont = tclSqlResult.Font;
                 var initialText = tclSqlResult.TabPages.Count > 0 ? tclSqlResult.TabPages[0].Text : "Messages";
                 var textSize = TextRenderer.MeasureText(g, initialText, tabFont);
                 const int horizontalPadding = 4 + 4;
                 const int buttonSpace = 20;
                 const int minWidth = 50;
                 
                 initialWidth = Math.Max(textSize.Width + horizontalPadding + buttonSpace, minWidth);
                 initialHeight = textSize.Height + 4 + 4;
            }
            tclSqlResult.ItemSize = new Size(initialWidth, initialHeight); 

            tclSqlResult.DrawItem += (s, e) =>
            {
                if (e.Index < 0 || e.Index >= tclSqlResult.TabCount) return;

                var tp = tclSqlResult.TabPages[e.Index];
                var g = e.Graphics;
                var tabRect = tclSqlResult.GetTabRect(e.Index);
                var isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
                var pal = UiThemeManager.Current;

                Color backColor;
                Color textColor;

                if (pal.IsDark)
                {
                    backColor = isSelected ? pal.HotBackground : pal.Background;
                    textColor = pal.Text;
                }
                else
                {
                    backColor = isSelected ? SystemColors.Highlight : SystemColors.Control;
                    textColor = isSelected ? SystemColors.HighlightText : SystemColors.ControlText;
                }

                using (var backBrush = new SolidBrush(backColor))
                {
                    g.FillRectangle(backBrush, tabRect);
                }

                Image buttonImage = e.Index == 0
                    ? Resources.gui_eraser1
                    : Resources.x_letter1;

                var buttonRect = GetTabButtonRect(e.Index);

                if (tabRect.Width > buttonRect.Width + 8)
                {
                    g.DrawImage(buttonImage, buttonRect);
                }

                var textRect = new Rectangle(
                    tabRect.Left + 4,
                    tabRect.Top + 2,
                    tabRect.Width - 8 - buttonRect.Width - 4,
                    tabRect.Height - 4);

                TextRenderer.DrawText(
                    g,
                    tp.Text,
                    e.Font,
                    textRect,
                    textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            };

            tclSqlResult.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Middle)
                {
                    var tabIndex = GetTabIndexAt(e.Location);
                    if (tabIndex > 0)
                    {
                        CloseResultTab(tabIndex);
                    }

                    return;
                }

                if (e.Button != MouseButtons.Left)
                    return;

                for (var i = 0; i < tclSqlResult.TabPages.Count; i++)
                {
                    if (!GetTabButtonRect(i).Contains(e.Location))
                        continue;

                    if (i == 0)
                    {
                        btnCloseAllResultWindows?.Clear();
                    }
                    else
                    {
                        CloseResultTab(i);
                    }

                    return;
                }

                if (tclSqlResult.SelectedIndex >= 0 && tclSqlResult.SelectedIndex != _selectedTabIndex)
                {
                    _selectedTabIndex = tclSqlResult.SelectedIndex;
                }
            };

            tclSqlResult.MouseClick += (s, e) =>
            {
                if (e.Button != MouseButtons.Right) return;

                var tabIndex = GetTabIndexAt(e.Location);
                if (tabIndex < 0) return;

                var menu = new ContextMenu();

                if (tabIndex == 0)
                {
                    menu.MenuItems.Add("Clear messages", (ss, ee) =>
                    {
                        btnCloseAllResultWindows?.Clear();
                    });
                }
                else
                {
                    menu.MenuItems.Add("Close this result tab", (ss, ee) =>
                    {
                        CloseResultTab(tabIndex);
                    });
                    menu.MenuItems.Add("Close other result tabs", (ss, ee) =>
                    {
                        CloseOtherResultTabs(tabIndex);
                    });
                }

                menu.MenuItems.Add("Close all result tabs", (ss, ee) =>
                {
                    CloseAllResultTabs();
                });
                menu.Show(tclSqlResult, e.Location);
            };
            
            InitResizeGrip();
        }
        
        private Rectangle GetTabButtonRect(int tabIndex)
        {
            var tabRect = tclSqlResult.GetTabRect(tabIndex);

            var drawnIconWidth = tabIndex == 0 ? 16 : 8;
            var drawnIconHeight = tabIndex == 0 ? 16 : 8;

            var buttonY = tabRect.Top + (tabRect.Height - drawnIconHeight) / 2;
            var buttonX = tabRect.Right - drawnIconWidth - 4;

            return new Rectangle(buttonX, buttonY, drawnIconWidth, drawnIconHeight);
        }

        private int GetTabIndexAt(Point location)
        {
            for (var i = 0; i < tclSqlResult.TabPages.Count; i++)
            {
                if (tclSqlResult.GetTabRect(i).Contains(location))
                {
                    return i;
                }
            }

            return -1;
        }

        private void CloseResultTab(int tabIndex)
        {
            CloseResultTab(tabIndex, true);
        }

        private void CloseResultTab(int tabIndex, bool selectNextTab)
        {
            if (tabIndex <= 0 || tabIndex >= tclSqlResult.TabPages.Count)
                return;

            var selectedTab = tclSqlResult.SelectedTab;
            var tabPage = tclSqlResult.TabPages[tabIndex];
            var selectedTabWasClosed = selectedTab == tabPage;

            tclSqlResult.TabPages.RemoveAt(tabIndex);
            tabPage.Dispose();

            if (selectNextTab)
            {
                if (!selectedTabWasClosed && selectedTab != null && tclSqlResult.TabPages.Contains(selectedTab))
                {
                    tclSqlResult.SelectTab(selectedTab);
                }
                else
                {
                    var nextIndexToSelect = Math.Max(0, Math.Min(tabIndex - 1, tclSqlResult.TabPages.Count - 1));
                    if (tclSqlResult.TabPages.Count > 0)
                    {
                        tclSqlResult.SelectedIndex = nextIndexToSelect;
                    }
                }

                _selectedTabIndex = tclSqlResult.SelectedIndex;
            }
        }
        
        private void InitResizeGrip()
        {
            _resizeGrip = new Panel
            {
                Dock = DockStyle.Top,
                Height = 4,
                Cursor = Cursors.SizeNS,
                BackColor = SystemColors.ControlDark
            };

            Controls.Add(_resizeGrip);
            _resizeGrip.BringToFront();

            _resizeGrip.MouseDown += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                _isResizing = true;
                _resizeStartScreenY = MousePosition.Y;
                _resizeStartHeight = Height;
                _resizeGrip.Capture = true;
            };

            _resizeGrip.MouseMove += (s, e) =>
            {
                if (!_isResizing) return;
                var delta = MousePosition.Y - _resizeStartScreenY;
                var requestedHeight = _resizeStartHeight - delta;
                UserResizeRequested?.Invoke(this, requestedHeight);
            };

            _resizeGrip.MouseUp += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                _isResizing = false;
                _resizeGrip.Capture = false;
            };

            _resizeGrip.MouseCaptureChanged += (s, e) =>
            {
                if (_resizeGrip.Capture) return;
                _isResizing = false;
            };
        }

        
        private static void Numbering(DataGridView dgv)
        {
            var idx = 1;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                row.HeaderCell.Value = idx++.ToString();
            }
        }
        
        public void SetAnalysisStatus(bool hasErrors, bool hasWarnings, int firstIssueLine = -1)
        {
            if (lblError == null) return;

            string statusMessage;
            Color statusColor;
            var showStatus = false;

            if (hasErrors)
            {
                statusMessage = "Analysis found errors.";
                statusColor = Color.Red;
                showStatus = true;
                if (firstIssueLine > 0)
                {
                    statusMessage += $" First error near L{firstIssueLine}.";
                }
            }
            else if (hasWarnings)
            {
                statusMessage = "Analysis found warnings.";
                statusColor = Color.Orange;
                showStatus = true;
                if (firstIssueLine > 0)
                {
                    statusMessage += $" First warning near L{firstIssueLine}.";
                }
            }
            else
            {
                statusMessage = "Analysis complete. No issues found.";
                statusColor = SystemColors.ControlText;
            }

            if (showStatus)
            {
                lblError.Text = statusMessage;
                lblError.ForeColor = statusColor;
                lblError.Visible = true;
                tspMain.Visible = false;
                tclSqlResult.Visible = false;
            }
            else
            {
                lblError.Visible = false;
                tspMain.Visible = true;
                tclSqlResult.Visible = true;
            }
        }
        
        public void ClearAnalysisStatus()
        {
            if (lblError == null) return;

            SetError(string.Empty);
            lblError.Text = string.Empty;
            lblError.Visible = false;

            tspMain.Visible = true;
            tclSqlResult.Visible = true;
        }

        private static void AdjustResizeColumnRow(DataGridView dgv)
        {
            dgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            dgv.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }

        public IDbConnect LinkedDbConnect { get; private set; }

        private ISqlExecutor _exec;
        private void SetConnect(IDbConnect connect, ISqlExecutor sqlExecutor)
        {
            if (_exec == null)
            {
                _exec = sqlExecutor;
            }
            LinkedDbConnect = connect;

            lblConnect.Text = $@"{connect.DatabaseSystemName}: {connect.Title}";

            if (connect.DatabaseSystemName != null && connect.DatabaseSystemName.StartsWith("PostgreSQL", StringComparison.OrdinalIgnoreCase))
            {
                lblAccount.Text = $@"Username: {connect.Account}";
                lblAccount.Visible = !string.IsNullOrEmpty(connect.Account);
                sepAccount.Visible = lblAccount.Visible;
            }
            else
            {
                lblAccount.Text = "";
                lblAccount.Visible = false;
                sepAccount.Visible = false;
            }

            if (connect is PostgreSqlConnect pgConnect)
            {
                lblDatabase.Text = $@"Database: {pgConnect.Database}";
                lblDatabase.Visible = !string.IsNullOrEmpty(pgConnect.Database);
                sepDatabase.Visible = lblDatabase.Visible;
            }
            else
            {
                lblDatabase.Text = "";
                lblDatabase.Visible = false;
                sepDatabase.Visible = false;
            }

            lblElapsed.Text = "";
            btnStop.Enabled = false;
        }

        public void SetError(string message)
        {
            if (lblError == null) return;

            if (string.IsNullOrEmpty(message))
            {
                lblError.Visible = false;
                tspMain.Visible = true;
                tclSqlResult.Visible = true;
                lblError.Text = "";
            }
            else
            {
                lblError.Text = message;
                lblError.ForeColor = UiThemeManager.IsDark ? Color.IndianRed : Color.Brown;
                lblError.Visible = true;
                tspMain.Visible = false;
                tclSqlResult.Visible = false;
            }
        }

        public ParserResult Parse(string sql, CaretPosition caretPosition)
        {
            return _exec.Parse(sql, caretPosition);
        }

        private void AddResultTabPage(int index, DataTable dataSource, string titleText, string toolTipText)
        {
            int requiredWidth;
            using (var g = CreateGraphics())
            {
                var tabFont = tclSqlResult.Font;
                var textSize = TextRenderer.MeasureText(g, titleText, tabFont);
                const int horizontalPadding = 4 + 4;
                const int buttonSpace = 20;
                const int minWidth = 50;
                requiredWidth = Math.Max(textSize.Width + horizontalPadding + buttonSpace, minWidth);
            }

            var currentWidth = tclSqlResult.ItemSize.Width;
            if (requiredWidth > currentWidth)
            {
                tclSqlResult.ItemSize = new Size(requiredWidth, tclSqlResult.ItemSize.Height);
            }

            var tp = new TabPage();
            var dgv = new DataGridView();
            
            var pal = UiThemeManager.Current;

            tp.SuspendLayout();
            ((ISupportInitialize)dgv).BeginInit();

            tp.Text = titleText;
            tp.ToolTipText = toolTipText;

            tclSqlResult.TabPages.Add(tp);

            tp.Controls.Add(dgv);
            tp.Location = new Point(4, 22);
            tp.Margin = new Padding(0);
            tp.Name = $"tabResult{index}";
            tp.UseVisualStyleBackColor = true;

            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AutoSize = false;
            dgv.BackgroundColor = pal.PureBackground;
            dgv.BorderStyle = BorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(3);
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.DataSource = dataSource;
             foreach (DataGridViewColumn col in dgv.Columns)
             {
                 if (dataSource.Columns.Contains(col.HeaderText))
                 {
                    col.HeaderText = dataSource.Columns[col.HeaderText].Caption;
                 }
             }
            dgv.DefaultCellStyle = dataGridViewCellStyle2;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.Dock = DockStyle.Fill;
            dgv.GridColor = pal.Edge;
            dgv.Location = new Point(0, 0);
            dgv.Margin = new Padding(0, 3, 0, 0);
            dgv.Name = $"grdResult{index}";
            dgv.ReadOnly = true;
            dgv.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgv.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.RowHeadersWidth = 43;
            dgv.RowTemplate.Height = 23;
            dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgv.ShowEditingIcon = false;
            dgv.ShowRowErrors = false;
            dgv.VirtualMode = true;

            dgv.Sorted += (s, e) => { Numbering(dgv); dgv.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders); };
            dgv.DataBindingComplete += (s, e) => { Numbering(dgv); dgv.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders); };

            AttachCopySupport(dgv);

            ((ISupportInitialize)dgv).EndInit();
            tp.ResumeLayout(true);
            UiThemeManager.Apply(tp);

            tclSqlResult.PerformLayout();
            tclSqlResult.Invalidate(true);

            BeginInvoke(new Action(() =>
            {
                try
                {
                    AdjustResizeColumnRow(dgv);
                    dgv.Invalidate(true);
                    dgv.Update();
                }
                catch {}
            }));
        }

        private static void AttachCopySupport(DataGridView dgv)
        {
            dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dgv.KeyDown += (s, e) =>
            {
                if (!e.Control || e.KeyCode != Keys.C) return;

                if (!TryCopySelectedCells(dgv))
                {
                    TryCopyCurrentCell(dgv);
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
            };

            dgv.MouseDown += (s, e) =>
            {
                if (e.Button != MouseButtons.Right) return;

                var hit = dgv.HitTest(e.X, e.Y);
                if (hit.Type == DataGridViewHitTestType.Cell)
                {
                    var clickedCell = dgv[hit.ColumnIndex, hit.RowIndex];
                    if (!clickedCell.Selected)
                    {
                        dgv.ClearSelection();
                        clickedCell.Selected = true;
                        dgv.CurrentCell = clickedCell;
                    }
                }
            };

            dgv.MouseUp += (s, e) =>
            {
                if (e.Button != MouseButtons.Right) return;

                var menu = new ContextMenu();
                menu.MenuItems.Add("Copy cell value", (ss, ee) => { TryCopyCurrentCell(dgv); });
                menu.MenuItems.Add("Copy selection", (ss, ee) =>
                {
                    if (!TryCopySelectedCells(dgv))
                    {
                        TryCopyCurrentCell(dgv);
                    }
                });
                menu.Show(dgv, new Point(e.X, e.Y));
            };
        }

        private static bool TryCopyCurrentCell(DataGridView dgv)
        {
            if (dgv?.CurrentCell == null) return false;

            var value = dgv.CurrentCell.Value;
            Clipboard.SetText(value == null ? string.Empty : Convert.ToString(value));
            return true;
        }

        private static bool TryCopySelectedCells(DataGridView dgv)
        {
            if (dgv == null || dgv.SelectedCells.Count == 0) return false;

            var minRow = int.MaxValue;
            var maxRow = int.MinValue;
            var minCol = int.MaxValue;
            var maxCol = int.MinValue;

            foreach (DataGridViewCell cell in dgv.SelectedCells)
            {
                if (cell.RowIndex < minRow) minRow = cell.RowIndex;
                if (cell.RowIndex > maxRow) maxRow = cell.RowIndex;
                if (cell.ColumnIndex < minCol) minCol = cell.ColumnIndex;
                if (cell.ColumnIndex > maxCol) maxCol = cell.ColumnIndex;
            }

            if (minRow == int.MaxValue || minCol == int.MaxValue) return false;

            var buffer = new StringBuilder();

            for (var row = minRow; row <= maxRow; row++)
            {
                if (row > minRow)
                {
                    buffer.AppendLine();
                }

                for (var col = minCol; col <= maxCol; col++)
                {
                    if (col > minCol)
                    {
                        buffer.Append('	');
                    }

                    var cell = dgv[col, row];
                    if (!cell.Selected) continue;

                    var value = cell.Value;
                    if (value != null)
                    {
                        buffer.Append(Convert.ToString(value));
                    }
                }
            }

            Clipboard.SetText(buffer.ToString());
            return true;
        }

        public void Execute(IList<string> sqlQueries)
        {
            if (!_exec.CanExecute())
            {
                MessageBox.Show(@"There are tasks that are in a transactional state or have not ended.");
                return;
            }

            btnStop.Enabled = true;

            var startPoint = DateTime.Now;

            _exec.Execute(sqlQueries, results =>
            {
                Invoke(new Action(delegate
                {
                    var elapsed = (DateTime.Now - startPoint).ToString("c");
                    lblElapsed.Text = $@"Elapsed time: {elapsed}";

                    btnStop.Enabled = _exec.CanStop();

                    try
                    {
                        _selectedTabIndex = 0;
                        if (tclSqlResult.TabPages.Count == 1) _tabCounter = 1;
                        foreach (var result in results)
                        {
                            if (result.Error != null)
                            {
                                var additionalMessage = result.CommandText == null ? "" : $"\r\nThis error occurred while executing statement:\r\n{result.CommandText}";
                                var message = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {result.Error.GetType().Name}: {result.Error.Message}{additionalMessage}\r\n\r\n";
                                btnCloseAllResultWindows.SelectionFont = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold, GraphicsUnit.Point, 129);
                                btnCloseAllResultWindows.AppendText(message);
                                btnCloseAllResultWindows.SelectionFont = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
                            }
                            else if (result.RecordsAffected > 0 && result.QueryResult.Columns.Count == 0)
                            {
                                var message = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {result.RecordsAffected} rows affected by statement:\r\n{result.CommandText}\r\n\r\n";
                                btnCloseAllResultWindows.AppendText(message);
                            }
                            else if (result.QueryResult.Columns.Count == 0)
                            {
                                var message = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] Statement executed successfully:\r\n{result.CommandText}";
                                if (result.CommandMessage != null)
                                {
                                    message += $"\r\nStatement resulted in action: \r\n{result.CommandMessage}";
                                }
                                message += "\r\n\r\n";
                                btnCloseAllResultWindows.AppendText(message);
                            }
                            else
                            {
                                var tabIndex = _tabCounter++;
                                var message = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {result.QueryResult.Rows.Count} rows returned into \"Result {tabIndex}\" by statement:\r\n{result.CommandText}\r\n\r\n";
                                var title = $"{LinkedDbConnect?.Title ?? "DB"} {tabIndex} ({DateTime.Now:dd/MM/yyyy HH:mm:ss})";
                                var tooltip = $"{result.QueryResult.Rows.Count} rows returned by statement:\r\n{result.CommandText}";
                                btnCloseAllResultWindows.AppendText(message);
                                AddResultTabPage(tabIndex, result.QueryResult, title, tooltip);
                                _selectedTabIndex = tclSqlResult.TabPages.Count - 1;
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                        MessageBox.Show(ex1.Message);
                    }

                    tclSqlResult.SelectTab(_selectedTabIndex);
                }));
            });
        }

        private void btnCloseAllResultWindows_Click(object sender, EventArgs e)
        {
            try
            {
                CloseTabsInAllActiveInstances();
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"An error occurred while trying to close all result tabs: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}