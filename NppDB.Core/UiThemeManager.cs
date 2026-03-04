using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NppDB.Core
{
    public static class UiThemeManager
    {
        private static readonly object _lock = new object();
        private static UiThemePalette _palette = UiThemePalette.CreateLightDefault();
        private static readonly List<WeakReference<Control>> _roots = new List<WeakReference<Control>>();

        public static UiThemePalette Current
        {
            get { lock (_lock) return _palette; }
        }

        public static bool IsDark
        {
            get { lock (_lock) return _palette != null && _palette.IsDark; }
        }

        public static void SetPalette(UiThemePalette palette)
        {
            if (palette == null) palette = UiThemePalette.CreateLightDefault();

            List<Control> live;
            lock (_lock)
            {
                _palette = palette;

                live = new List<Control>();
                _roots.RemoveAll(wr => !wr.TryGetTarget(out var c) || c == null || c.IsDisposed);
                foreach (var wr in _roots)
                    if (wr.TryGetTarget(out var c) && c != null && !c.IsDisposed)
                        live.Add(c);
            }

            foreach (var root in live)
            {
                try { Apply(root); } catch { }
            }
        }

        public static void Register(Control root)
        {
            if (root == null) return;

            lock (_lock)
            {
                _roots.RemoveAll(wr => !wr.TryGetTarget(out var c) || c == null || c.IsDisposed);
                if (!_roots.Any(wr => wr.TryGetTarget(out var c) && ReferenceEquals(c, root)))
                    _roots.Add(new WeakReference<Control>(root));
            }

            Apply(root);
        }

        public static void Apply(Control root)
        {
            if (root == null || root.IsDisposed) return;

            UiThemePalette p;
            lock (_lock) p = _palette ?? UiThemePalette.CreateLightDefault();

            ApplyRecursive(root, p);

            try
            {
                root.Invalidate(true);
                root.Update();
            }
            catch { }
        }

        private static void ApplyRecursive(Control c, UiThemePalette p)
        {
            if (c == null || c.IsDisposed) return;

            ApplyOne(c, p);

            foreach (Control child in c.Controls)
                ApplyRecursive(child, p);

            if (c is SplitContainer sc)
            {
                ApplyRecursive(sc.Panel1, p);
                ApplyRecursive(sc.Panel2, p);
            }
        }

        private static void ApplyOne(Control c, UiThemePalette p)
        {
            if (c is ToolStrip ts)
            {
                ts.Renderer = new ToolStripProfessionalRenderer(new UiThemeColorTable(p));
                ts.BackColor = p.Background;
                ts.ForeColor = p.Text;
                return;
            }

            if (c is DataGridView dgv)
            {
                ApplyDataGridView(dgv, p);
                return;
            }

            if (c is TreeView tv)
            {
                tv.BackColor = p.PureBackground;
                tv.ForeColor = p.Text;
                return;
            }

            if (c is TextBoxBase)
            {
                c.BackColor = p.PureBackground;
                c.ForeColor = p.Text;
                return;
            }

            if (c is ComboBox cb)
            {
                cb.BackColor = p.PureBackground;
                cb.ForeColor = p.Text;
                return;
            }

            if (c is NumericUpDown nud)
            {
                nud.BackColor = p.PureBackground;
                nud.ForeColor = p.Text;
                return;
            }

            if (c is LinkLabel ll)
            {
                ll.LinkColor = p.LinkText;
                ll.ActiveLinkColor = p.LinkText;
                ll.VisitedLinkColor = p.LinkText;
                ll.ForeColor = p.Text;
                return;
            }

            if (c is Button btn)
            {
                btn.BackColor = p.SofterBackground;
                btn.ForeColor = p.Text;
                return;
            }

            if (c is CheckBox || c is RadioButton)
            {
                c.BackColor = p.Background;
                c.ForeColor = p.Text;
                return;
            }
            
            if (c is TabControl tc)
            {
                tc.BackColor = p.Background;
                tc.ForeColor = p.Text;
                return;
            }

            if (c is TabPage tp)
            {
                tp.UseVisualStyleBackColor = false;
                tp.BackColor = p.Background;
                tp.ForeColor = p.Text;
                return;
            }

            if (c is GroupBox gb)
            {
                gb.BackColor = p.Background;
                gb.ForeColor = p.Text;
                return;
            }

            if (c is Label lab)
            {
                lab.ForeColor = p.Text;
                return;
            }

            c.BackColor = p.Background;
            c.ForeColor = p.Text;
        }

        private static void ApplyDataGridView(DataGridView dgv, UiThemePalette p)
        {
            dgv.EnableHeadersVisualStyles = false;

            dgv.BackgroundColor = p.PureBackground;
            dgv.GridColor = p.Edge;

            dgv.DefaultCellStyle.BackColor = p.PureBackground;
            dgv.DefaultCellStyle.ForeColor = p.Text;
            dgv.DefaultCellStyle.SelectionBackColor = p.HotBackground;
            dgv.DefaultCellStyle.SelectionForeColor = p.Text;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = p.Background;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = p.Text;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = p.SofterBackground;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = p.Text;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = p.SofterBackground;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = p.Text;

            dgv.RowHeadersDefaultCellStyle.BackColor = p.SofterBackground;
            dgv.RowHeadersDefaultCellStyle.ForeColor = p.Text;
        }
    }
}