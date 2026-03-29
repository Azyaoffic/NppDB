using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NppDB.Comm;
using NppDB.Core.Properties;
using NppDB.MSAccess;
using NppDB.PostgreSQL;

namespace NppDB.Core
{
    public class DbTemplateContext
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public string Dialect { get; set; }
        public string ColumnsWithTypes { get; set; }
    }
    
    

    public partial class FrmDatabaseExplore : Form
    {
        private sealed class DbTreeViewState
        {
            public string SelectedPath { get; set; }
            public List<string> ExpandedPaths { get; set; } = new List<string>();
        }

        private readonly Dictionary<IntPtr, DbTreeViewState> _treeStatesByBuffer = new Dictionary<IntPtr, DbTreeViewState>();
        private readonly Dictionary<TreeNode, string> _lastSelectedTablePathByRoot = new Dictionary<TreeNode, string>();
        private bool _isRestoringTreeState;

        private readonly INppDbCommandHost _commandHostInstance;
        private bool _dbManagerFontScaleApplied;

        public FrmDatabaseExplore(INppDbCommandHost commandHost)
        {
            InitializeComponent();
            UiThemeManager.Register(this);
            _commandHostInstance = commandHost;
            Init();
        }

        private void Init()
        {
            try
            {
                ApplyDbManagerFontScale();

                InitTreeIcons();

                if (DbServerManager.Instance == null)
                {
                    return;
                }
                if (DbServerManager.Instance.Connections == null)
                {
                    return;
                }

                foreach (var dbcnn in DbServerManager.Instance.Connections)
                {
                    if (!(dbcnn is TreeNode node))
                    {
                         continue;
                    }

                    var dbTypes = DbServerManager.Instance.GetDatabaseTypes();

                    var dbType = dbTypes?.FirstOrDefault(x => x != null && x.ConnectType == dbcnn.GetType());
                    if (dbType == null) continue;
                    SetTreeNodeImage(node, dbType.Id);
                    trvDBList.Nodes.Add(node);
                }

                btnRegister.Enabled = true;
                btnUnregister.Enabled = false;
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = false;
                btnRefresh.Enabled = false;

                if (trvDBList.TopNode != null)
                {
                    trvDBList.ItemHeight = trvDBList.TopNode.Bounds.Height + 4;
                }

                trvDBList.ShowNodeToolTips = true;
                trvDBList.BeforeExpand += trvDBList_BeforeExpand;
                
                UpdateCurrentTabConnectionLabel(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CRITICAL Error during FrmDatabaseExplore.Init():\nMessage: {ex.Message}\nStackTrace:\n{ex.StackTrace}",
                                "FrmDatabaseExplore Init CRASH", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ApplyDbManagerFontScale()
        {
            if (_dbManagerFontScaleApplied) return;
            _dbManagerFontScaleApplied = true;

            var scale = NppDbSettingsStore.Get().Behavior.DbManagerFontScale;

            if (scale < 0.75f || scale > 2.5f) scale = 1.0f;
            if (Math.Abs(scale - 1.0f) < 0.01f) return;

            if (trvDBList == null) return;

            trvDBList.Font = new Font(trvDBList.Font.FontFamily, trvDBList.Font.Size * scale, trvDBList.Font.Style);
            trvDBList.ItemHeight = (int)Math.Round(trvDBList.Font.Height * 1.35) + 2;
        }
        
        private void InitTreeIcons()
        {
            trvDBList.ImageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit };
            trvDBList.ImageList.Images.Add("Database", Resources.Database);
            
            AddTreeIcon("Database", Resources.Database);
            AddTreeIcon("Schema", Resources.Folder);
            AddTreeIcon("Group", Resources.Folder);
            AddTreeIcon("Table", Resources.Table);

            AddTreeIcon("View", Resources.page_file);
            AddTreeIcon("MaterializedView", Resources.page_file);
            AddTreeIcon("Function", Resources.bullet);
            AddTreeIcon("ForeignTable", Resources.shortcuts6);

            AddTreeIcon("Primary_Key", Resources.primaryKey);
            AddTreeIcon("Foreign_Key", Resources.foreignKey);
            AddTreeIcon("Index", Resources.index);
            AddTreeIcon("Unique_Index", Resources.uniqueIndex);
            
            for (var fk = 0; fk <= 1; fk++)
            for (var pk = 0; pk <= 1; pk++)
            for (var idx = 0; idx <= 1; idx++)
            for (var nn = 0; nn <= 1; nn++)
            {
                var suffix = $"{fk}{pk}{idx}{nn}";
                var bmp = Resources.ResourceManager.GetObject("column" + suffix) as Bitmap;
                AddTreeIcon("Column_" + suffix, bmp);
            }
        }
        
        private static string GetNodeStateKey(TreeNode node)
        {
            return node.GetType().FullName + "|" + node.Text;
        }

        private static string GetNodeStatePath(TreeNode node)
        {
            var parts = new List<string>();

            while (node != null)
            {
                parts.Add(GetNodeStateKey(node));
                node = node.Parent;
            }

            parts.Reverse();
            return string.Join("\u001F", parts);
        }

        private static TreeNode FindNodeByStateKey(TreeNodeCollection nodes, string stateKey)
        {
            foreach (TreeNode node in nodes)
            {
                if (GetNodeStateKey(node) == stateKey)
                    return node;
            }

            return null;
        }

        private TreeNode FindNodeByStatePath(string statePath, bool expandParents)
        {
            if (string.IsNullOrWhiteSpace(statePath))
                return null;

            var parts = statePath.Split(new[] { "\u001F" }, StringSplitOptions.None);
            if (parts.Length == 0)
                return null;

            TreeNode current = FindNodeByStateKey(trvDBList.Nodes, parts[0]);
            if (current == null)
                return null;

            for (var i = 1; i < parts.Length; i++)
            {
                if (expandParents && !current.IsExpanded)
                    current.Expand();

                current = FindNodeByStateKey(current.Nodes, parts[i]);
                if (current == null)
                    return null;
            }

            return current;
        }

        private static void CollectExpandedPaths(TreeNode node, List<string> expandedPaths)
        {
            if (node == null)
                return;

            if (node.IsExpanded)
                expandedPaths.Add(GetNodeStatePath(node));

            foreach (TreeNode child in node.Nodes)
            {
                CollectExpandedPaths(child, expandedPaths);
            }
        }

        private DbTreeViewState CaptureCurrentTreeState()
        {
            var state = new DbTreeViewState();

            if (trvDBList.SelectedNode != null)
                state.SelectedPath = GetNodeStatePath(trvDBList.SelectedNode);

            foreach (TreeNode root in trvDBList.Nodes)
            {
                CollectExpandedPaths(root, state.ExpandedPaths);
            }

            return state;
        }
        
        private static DbTreeViewState CloneTreeState(DbTreeViewState state)
        {
            if (state == null)
                return null;

            return new DbTreeViewState
            {
                SelectedPath = state.SelectedPath,
                ExpandedPaths = state.ExpandedPaths != null
                    ? new List<string>(state.ExpandedPaths)
                    : new List<string>()
            };
        }

        public void CloneTreeStateForBuffer(IntPtr sourceBufferId, IntPtr targetBufferId, bool restoreTarget)
        {
            if (sourceBufferId == IntPtr.Zero || targetBufferId == IntPtr.Zero)
                return;

            if (!_treeStatesByBuffer.TryGetValue(sourceBufferId, out var state) || state == null)
                return;

            _treeStatesByBuffer[targetBufferId] = CloneTreeState(state);

            if (restoreTarget)
                RestoreTreeStateForBuffer(targetBufferId);
        }
        
        public void SaveTreeStateForBuffer(IntPtr bufferId)
        {
            if (bufferId == IntPtr.Zero)
                return;

            _treeStatesByBuffer[bufferId] = CaptureCurrentTreeState();
        }

        public void RemoveTreeStateForBuffer(IntPtr bufferId)
        {
            if (bufferId == IntPtr.Zero)
                return;

            _treeStatesByBuffer.Remove(bufferId);
        }

        public void RestoreTreeStateForBuffer(IntPtr bufferId)
        {
            if (bufferId == IntPtr.Zero)
                return;

            _isRestoringTreeState = true;
            trvDBList.BeginUpdate();

            try
            {
                trvDBList.SelectedNode = null;
                trvDBList.CollapseAll();

                if (!_treeStatesByBuffer.TryGetValue(bufferId, out var state) || state == null)
                {
                    UpdateToolbarState(null);
                    return;
                }

                foreach (var expandedPath in state.ExpandedPaths
                             .OrderBy(x => x.Count(c => c == '\u001F')))
                {
                    var node = FindNodeByStatePath(expandedPath, true);
                    node?.Expand();
                }

                if (!string.IsNullOrWhiteSpace(state.SelectedPath))
                {
                    var selectedNode = FindNodeByStatePath(state.SelectedPath, true);
                    if (selectedNode != null)
                    {
                        trvDBList.SelectedNode = selectedNode;
                        selectedNode.EnsureVisible();
                    }
                }

                UpdateToolbarState(trvDBList.SelectedNode);
            }
            finally
            {
                trvDBList.EndUpdate();
                _isRestoringTreeState = false;
            }
        }
        
        
        private void AddTreeIcon(string key, Bitmap icon)
        {
            if (icon == null) throw new ArgumentNullException(nameof(icon), $"Icon for key '{key}' is null.");
            if (!trvDBList.ImageList.Images.ContainsKey(key))
                trvDBList.ImageList.Images.Add(key, icon);
        }

        private readonly List<NotifyHandler> _notifyHandlers = new List<NotifyHandler>();
        public void AddNotifyHandler(NotifyHandler handler)
        {
            _notifyHandlers.Add(handler);
        }

        protected override void WndProc(ref Message m)
        {
            if (_notifyHandlers.Count > 0 && m.Msg == 0x4e)
                foreach (var hnd in _notifyHandlers)
                    hnd(ref m);
            
            base.WndProc(ref m);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                RegisterConnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + (ex.InnerException != null ? " : " + ex.InnerException.Message : ""));
            }
        }

        private void RegisterConnect()
        {
            var dlg = new FrmSelectDbType();
            if (dlg.ShowDialog(this) != DialogResult.OK) return;
            var selDbType = dlg.SelectedDatabaseType;
            var dbConnection = DbServerManager.Instance.CreateConnect(selDbType);

            bool checkLoginResult;
            try
            {
                checkLoginResult = dbConnection.CheckLogin();
            }
            catch (Exception exCheckLogin)
            {
                MessageBox.Show($"RegisterConnect: UNEXPECTED ERROR during CheckLogin call: {exCheckLogin.Message}", @"Debug RegisterConnect Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                checkLoginResult = false;
            }

            if (!checkLoginResult)
            {
                return;
            }

            var tmpName = dbConnection.GetDefaultTitle();
            var existingCount = DbServerManager.Instance.Connections.Count(x => x.Title.StartsWith(tmpName));
            dbConnection.Title = tmpName + (existingCount == 0 ? "" : "(" + existingCount + ")");

            DbServerManager.Instance.Register(dbConnection);
            var id = selDbType.Id;
            var node = dbConnection as TreeNode;
            if (node != null)
            {
                SetTreeNodeImage(node, id);
                trvDBList.Nodes.Add(node);

                if (trvDBList.TopNode != null && trvDBList.ItemHeight != trvDBList.TopNode.Bounds.Height + 4)
                    trvDBList.ItemHeight = trvDBList.TopNode.Bounds.Height + 4;
            }

            try
            {
                dbConnection.Connect();
                dbConnection.Attach();
                dbConnection.Refresh();
                node?.Expand();
            }
            catch (Exception exConnect)
            {
                MessageBox.Show($"RegisterConnect: ERROR during Connect/Attach/Refresh: {exConnect.Message}" + (exConnect.InnerException != null ? " Inner: " + exConnect.InnerException.Message : ""), @"Debug RegisterConnect Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetTreeNodeImage(TreeNode node, string id)
        {
            var iconProvider = node as IIconProvider;
            if(!trvDBList.ImageList.Images.ContainsKey(id)) 
                trvDBList.ImageList.Images.Add(id, iconProvider.GetIcon());
            node.SelectedImageKey = node.ImageKey = id;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!(trvDBList.SelectedNode is IDbConnect connector)) return;
            try
            {
                var result = connector.ConnectAndAttach();
                if (result != "CONTINUE") { return; }
                trvDBList.SelectedNode.Expand();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + (ex.InnerException != null ? " : " + ex.InnerException.Message: ""));
            }
        }
        
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (!(trvDBList.SelectedNode is IDbConnect connection) || trvDBList.SelectedNode.Level > 0) return;

            _lastSelectedTablePathByRoot.Remove(trvDBList.SelectedNode);

            DisconnectHandler?.Invoke(connection);
            trvDBList.SelectedNode?.Nodes.Clear();
        }

        private void btnUnregister_Click(object sender, EventArgs e)
        {
            if (!(trvDBList.SelectedNode is IDbConnect connection) || trvDBList.SelectedNode.Level > 0) return;

            _lastSelectedTablePathByRoot.Remove(trvDBList.SelectedNode);

            trvDBList.Nodes.Remove(trvDBList.SelectedNode);
            btnRegister.Enabled = true;
            btnUnregister.Enabled = false;
            btnConnect.Enabled = false;
            btnDisconnect.Enabled = false;
            btnRefresh.Enabled = false;

            UnregisterHandler?.Invoke(connection);
        }
        
        private void UpdateToolbarState(TreeNode node)
        {
            btnUnregister.Enabled = node is IDbConnect;

            var dbConnection = node != null ? GetRootParent(node) as IDbConnect : null;
            if (dbConnection == null)
            {
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = false;
                btnRefresh.Enabled = false;
                return;
            }

            btnConnect.Enabled = node is IDbConnect && !dbConnection.IsOpened;
            btnDisconnect.Enabled = node is IDbConnect && dbConnection.IsOpened;
            btnRefresh.Enabled = node is IRefreshable && dbConnection.IsOpened;
        }

        private TreeNode FindConnectionNode(IDbConnect connection)
        {
            if (connection == null)
                return null;

            foreach (TreeNode node in trvDBList.Nodes)
            {
                if (node is IDbConnect dbConnect && ReferenceEquals(dbConnect, connection))
                    return node;
            }

            return null;
        }

        private void UpdateCurrentTabConnectionLabel(IDbConnect connection)
        {
            if (connection == null)
            {
                lblCurrentTabConnection.Text = "Current tab connection: none";
                return;
            }

            var dbSystem = string.IsNullOrWhiteSpace(connection.DatabaseSystemName)
                ? "Database"
                : connection.DatabaseSystemName;

            var title = string.IsNullOrWhiteSpace(connection.Title)
                ? "(unnamed connection)"
                : connection.Title;

            lblCurrentTabConnection.Text = $"Current tab connection: {dbSystem}: {title}";
        }

        public void SyncCurrentTabConnection(IDbConnect connection)
        {
            UpdateCurrentTabConnectionLabel(connection);

            var selectedNode = trvDBList.SelectedNode;
            var selectedRootConnection = selectedNode != null ? GetRootParent(selectedNode) as IDbConnect : null;

            if (selectedNode == null)
            {
                UpdateToolbarState(null);
                return;
            }

            if (connection == null)
            {
                UpdateToolbarState(selectedNode);
                return;
            }

            if (ReferenceEquals(selectedRootConnection, connection))
            {
                UpdateToolbarState(selectedNode);
                return;
            }

            UpdateToolbarState(selectedNode);
        }

        private void trvDBList_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count != 1 || !string.IsNullOrEmpty(e.Node.Nodes[0].Text)) return;
            if (e.Node is IRefreshable refreshableNode)
            {
                Cursor = Cursors.WaitCursor;
                trvDBList.Cursor = Cursors.WaitCursor;
                try
                {
                    refreshableNode.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error expanding node '{e.Node.Text}':\n{ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
                finally
                {
                    Cursor = Cursors.Default;
                    trvDBList.Cursor = Cursors.Default;
                }
            }
            else
            {
                e.Node.Nodes.Clear();
            }
        }

        public delegate void DatabaseEventHandler(IDbConnect connection);

        public DatabaseEventHandler DisconnectHandler { get; set; }
        public DatabaseEventHandler UnregisterHandler { get; set; }
        
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (!(trvDBList.SelectedNode is IRefreshable refreshable)) return;
            refreshable.Refresh();
        }

        private void trvDBList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RememberLastSelectedTable(e.Node);
            UpdateToolbarState(e.Node);
        }

        private void trvDBList_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            trvDBList.SelectedNode = e.Node;
            e.Node.ContextMenuStrip = CreateMenu(e.Node);
        }

        private static TreeNode GetRootParent(TreeNode node)
        {
            while (node.Parent != null) node = node.Parent;
            return node;
        }

        private void trvDBList_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            IRefreshable r;
            if (e.Button != MouseButtons.Left || (r = e.Node as IRefreshable) == null) return;
            var dbConnection = GetRootParent(e.Node) as IDbConnect;
            if ( e.Node.Nodes.Count == 0)
            {
                if (dbConnection != null)
                {
                    var result = dbConnection.ConnectAndAttach();
                    if (result != "CONTINUE") { return;  }
                }

                r.Refresh();
            }
            e.Node.Expand();
        }

        private ContextMenuStrip CreateMenu(TreeNode node)
        {
            var menuCreator = node as IMenuProvider;
            var menu = menuCreator?.GetMenu() ?? new ContextMenuStrip { ShowImageMargin = false };

            var insertIndex = 0;

            if (node is PostgreSqlConnect)
            {
                menu.Items.Insert(insertIndex++, new ToolStripButton("Edit database connection details", null, EditConnection_Click));
                menu.Items.Insert(insertIndex++, new ToolStripSeparator());
            }

            if (node is IDbConnect connection)
            {
                menu.Items.Insert(insertIndex++, new ToolStripButton("Remove this database connection", null, btnUnregister_Click));
                menu.Items.Insert(insertIndex++, new ToolStripSeparator());
                menu.Items.Insert(insertIndex++, new ToolStripButton("Connect to this database", null, btnConnect_Click) { Enabled = !connection.IsOpened });
                menu.Items.Insert(insertIndex++, new ToolStripButton("Disconnect from this database", null, btnDisconnect_Click) { Enabled = connection.IsOpened });
                menu.Items.Insert(insertIndex++, new ToolStripSeparator());
            }

            if (menu.Items.Count <= 0) return menu;
            if (menu.Items[menu.Items.Count - 1] is ToolStripSeparator && (menuCreator?.GetMenu() == null || menuCreator.GetMenu().Items.Count == 0))
            {
                menu.Items.RemoveAt(menu.Items.Count - 1);
            }
            if (menu.Items.Count > 0 && menu.Items[0] is ToolStripSeparator && insertIndex == menu.Items.Count && menuCreator?.GetMenu() == null)
            {
                menu.Items.RemoveAt(0);
            }


            return menu;
        }
        
        private void EditConnection_Click(object sender, EventArgs e)
        {
            if (trvDBList.SelectedNode is PostgreSqlConnect pgConnection)
            {
                try
                {
                    var changesMade = pgConnection.CheckLogin();

                    if (!changesMade) return;
                    trvDBList.SelectedNode.Text = pgConnection.Title;

                    if (pgConnection.IsOpened)
                    {
                        MessageBox.Show(this, "Connection properties have been updated.\n\nPlease disconnect and reconnect for these changes\nto take effect on the live database connection.",
                            @"Properties Changed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Error opening connection properties:\n{ex.Message}",
                                    @"Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(this, @"The selected item is not a PostgreSQL connection.", @"Cannot Edit Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void shortcuts_Click(object sender, EventArgs e)
        {
            var rows = new[]
            {
                new[] { "Execute SQL", "F9" },
                new[] { "Analyze SQL", "Shift+F9" },
                new[] { "Analyze and Create Prompt", "Ctrl+F9" },
                new[] { "Analyze and Create Prompt (Issue at Caret)", "Alt+F9" },
                new[] { "Clear Analysis", "Ctrl+Shift+F9" },
                new[] { "DB Connect Manager", "F10" },
                new[] { "Show Prompt Library", "Ctrl+F10" },
                new[] { "Show Tutorial", "Ctrl+F11" }
            };

            var leftWidth = rows.Max(r => r[0].Length) + 2;

            var sb = new StringBuilder();
            sb.AppendLine("NppDB Shortcuts");
            sb.AppendLine(new string('-', leftWidth + 16));

            foreach (var row in rows)
            {
                sb.AppendLine(row[0].PadRight(leftWidth) + row[1]);
            }

            using (var dlg = new Form())
            using (var txt = new TextBox())
            using (var btnOk = new Button())
            {
                dlg.Text = @"NppDB Shortcuts";
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MinimizeBox = false;
                dlg.MaximizeBox = false;
                dlg.ShowInTaskbar = false;
                dlg.ClientSize = new Size(500, 300);

                txt.ReadOnly = true;
                txt.Multiline = true;
                txt.ScrollBars = ScrollBars.Vertical;
                txt.WordWrap = false;
                txt.BorderStyle = BorderStyle.None;
                txt.Font = new Font("Consolas", 10F);
                txt.Text = sb.ToString();
                txt.Location = new Point(12, 12);
                txt.Size = new Size(dlg.ClientSize.Width - 24, dlg.ClientSize.Height - 60);
                txt.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                btnOk.Text = @"OK";
                btnOk.DialogResult = DialogResult.OK;
                btnOk.Size = new Size(90, 28);
                btnOk.Location = new Point(dlg.ClientSize.Width - btnOk.Width - 12, dlg.ClientSize.Height - btnOk.Height - 12);
                btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                dlg.AcceptButton = btnOk;
                dlg.Controls.Add(txt);
                dlg.Controls.Add(btnOk);

                dlg.ShowDialog(this);
            }
        }
    
        public DbTemplateContext GetCurrentTemplateContext()
        {
            var node = trvDBList.SelectedNode;
            if (node == null)
                return null;
        
            var context = new DbTemplateContext();
        
            var root = node;
            while (root.Parent != null)
                root = root.Parent;

            if (root is IDbConnect dbConn)
            {
                try
                {
                    var dialectRaw = dbConn.Dialect;
                    switch (dialectRaw)
                    {
                        case SqlDialect.NONE:
                            context.Dialect = "None";
                            break;
                        case SqlDialect.POSTGRE_SQL:
                            context.Dialect = "PostgreSQL";
                            break;
                        case SqlDialect.MS_ACCESS:
                            context.Dialect = "MS Access";
                            break;
                        default:
                            context.Dialect = $"Dialect_{dialectRaw}";
                            break;
                    }
                }
                catch
                {
                    context.Dialect = string.Empty;
                }

                if (dbConn is PostgreSqlConnect pgConn)
                {
                    context.DatabaseName = pgConn.Database;
                }
                else if (dbConn is MsAccessConnect msAccessConn)
                {
                    context.DatabaseName = Path.GetFileNameWithoutExtension(msAccessConn.ServerAddress);
                }

                if (string.IsNullOrWhiteSpace(context.DatabaseName))
                {
                    context.DatabaseName = dbConn.Title;
                }
            }

            var tableNode = GetEffectiveTableNode(node);

            if (tableNode is PostgreSqlTable postgreSqlTableNode)
            {
                context.TableName = postgreSqlTableNode.Text;
                EnsureTemplateMetadataLoaded(postgreSqlTableNode);
                context.ColumnsWithTypes = GetColumnsWithTypesFromTree(postgreSqlTableNode);
                return context;
            }

            if (tableNode is MsAccessTable msAccessTableNode)
            {
                context.TableName = msAccessTableNode.Text;
                EnsureTemplateMetadataLoaded(msAccessTableNode);
                context.ColumnsWithTypes = GetColumnsWithTypesFromTree(msAccessTableNode);
            }

            return context;
        }        
        private void RememberLastSelectedTable(TreeNode node)
        {
            if (node == null)
                return;

            var root = GetRootParent(node);
            if (root == null)
                return;

            TreeNode tableNode = GetParentPostgreSqlTableNode(node);
            if (tableNode == null)
                tableNode = GetParentMsAccessTableNode(node);

            if (tableNode == null)
                return;

            _lastSelectedTablePathByRoot[root] = GetNodeStatePath(tableNode);
        }

        private TreeNode GetEffectiveTableNode(TreeNode node)
        {
            if (node == null)
                return null;

            TreeNode tableNode = GetParentPostgreSqlTableNode(node);
            if (tableNode == null)
                tableNode = GetParentMsAccessTableNode(node);

            if (tableNode != null)
                return tableNode;

            var root = GetRootParent(node);
            if (root == null)
                return null;

            if (!_lastSelectedTablePathByRoot.TryGetValue(root, out var tablePath) || string.IsNullOrWhiteSpace(tablePath))
                return null;

            return FindNodeByStatePath(tablePath, true);
        }
        
        private static void EnsureTemplateMetadataLoaded(TreeNode tableNode)
        {
            if (tableNode == null)
                return;

            var hasOnlyPlaceholderChild =
                tableNode.Nodes.Count == 1 &&
                string.IsNullOrWhiteSpace(tableNode.Nodes[0].Text);

            var needsLoad = tableNode.Nodes.Count == 0 || hasOnlyPlaceholderChild;

            if (!needsLoad)
                return;

            if (tableNode is IRefreshable refreshableNode)
            {
                refreshableNode.Refresh();
            }
        }

        private static PostgreSqlTable GetParentPostgreSqlTableNode(TreeNode node)
        {
            while (node != null)
            {
                if (node is PostgreSqlTable tableNode)
                    return tableNode;

                node = node.Parent;
            }

            return null;
        }

        private static MsAccessTable GetParentMsAccessTableNode(TreeNode node)
        {
            while (node != null)
            {
                if (node is MsAccessTable tableNode)
                    return tableNode;

                node = node.Parent;
            }

            return null;
        }

        private static string GetColumnsWithTypesFromTree(TreeNode tableNode)
        {
            if (tableNode == null || tableNode.Nodes == null || tableNode.Nodes.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (TreeNode childNode in tableNode.Nodes)
            {
                if (childNode == null)
                    continue;

                sb.AppendLine(childNode.Text);
            }

            return sb.ToString().TrimEnd('\r', '\n');
        }
    }
}
