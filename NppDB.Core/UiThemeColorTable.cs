using System.Drawing;
using System.Windows.Forms;

namespace NppDB.Core
{
    internal sealed class UiThemeColorTable : ProfessionalColorTable
    {
        private readonly UiThemePalette _p;

        public UiThemeColorTable(UiThemePalette palette)
        {
            _p = palette ?? UiThemePalette.CreateLightDefault();
            UseSystemColors = false;
        }

        public override Color ToolStripGradientBegin => _p.Background;
        public override Color ToolStripGradientMiddle => _p.Background;
        public override Color ToolStripGradientEnd => _p.Background;

        public override Color ToolStripBorder => _p.Edge;

        public override Color ButtonSelectedHighlight => _p.HotBackground;
        public override Color ButtonSelectedBorder => _p.HotEdge;
        public override Color ButtonPressedHighlight => _p.HotBackground;
        public override Color ButtonPressedBorder => _p.HotEdge;

        public override Color MenuItemSelected => _p.HotBackground;
        public override Color MenuItemBorder => _p.HotEdge;

        public override Color SeparatorDark => _p.Edge;
        public override Color SeparatorLight => _p.Edge;

        public override Color ImageMarginGradientBegin => _p.Background;
        public override Color ImageMarginGradientMiddle => _p.Background;
        public override Color ImageMarginGradientEnd => _p.Background;
    }
}