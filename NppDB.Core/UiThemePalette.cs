using System.Drawing;

namespace NppDB.Core
{
    public sealed class UiThemePalette
    {
        public bool IsDark { get; set; }

        public Color Background { get; set; }
        public Color SofterBackground { get; set; }
        public Color HotBackground { get; set; }
        public Color PureBackground { get; set; }
        public Color ErrorBackground { get; set; }

        public Color Text { get; set; }
        public Color DarkerText { get; set; }
        public Color DisabledText { get; set; }
        public Color LinkText { get; set; }

        public Color Edge { get; set; }
        public Color HotEdge { get; set; }

        public static UiThemePalette CreateLightDefault()
        {
            return new UiThemePalette
            {
                IsDark = false,

                Background = SystemColors.Control,
                SofterBackground = SystemColors.ControlLight,
                HotBackground = SystemColors.Highlight,
                PureBackground = SystemColors.Window,
                ErrorBackground = Color.MistyRose,

                Text = SystemColors.ControlText,
                DarkerText = SystemColors.ControlDarkDark,
                DisabledText = SystemColors.GrayText,
                LinkText = SystemColors.HotTrack,

                Edge = SystemColors.ControlDark,
                HotEdge = SystemColors.Highlight
            };
        }

        public static UiThemePalette CreateDarkFallback()
        {
            return new UiThemePalette
            {
                IsDark = true,

                Background = Color.FromArgb(32, 32, 32),
                SofterBackground = Color.FromArgb(45, 45, 45),
                HotBackground = Color.FromArgb(60, 60, 60),
                PureBackground = Color.FromArgb(26, 26, 26),
                ErrorBackground = Color.FromArgb(70, 35, 35),

                Text = Color.Gainsboro,
                DarkerText = Color.Silver,
                DisabledText = Color.Gray,
                LinkText = Color.DeepSkyBlue,

                Edge = Color.FromArgb(64, 64, 64),
                HotEdge = Color.FromArgb(90, 90, 90)
            };
        }

        public static UiThemePalette FromNppDarkModeColors(
            uint background, uint softerBackground, uint hotBackground,
            uint pureBackground, uint errorBackground,
            uint text, uint darkerText, uint disabledText, uint linkText,
            uint edge, uint hotEdge)
        {
            return new UiThemePalette
            {
                IsDark = true,

                Background = ColorTranslator.FromWin32(unchecked((int)background)),
                SofterBackground = ColorTranslator.FromWin32(unchecked((int)softerBackground)),
                HotBackground = ColorTranslator.FromWin32(unchecked((int)hotBackground)),
                PureBackground = ColorTranslator.FromWin32(unchecked((int)pureBackground)),
                ErrorBackground = ColorTranslator.FromWin32(unchecked((int)errorBackground)),

                Text = ColorTranslator.FromWin32(unchecked((int)text)),
                DarkerText = ColorTranslator.FromWin32(unchecked((int)darkerText)),
                DisabledText = ColorTranslator.FromWin32(unchecked((int)disabledText)),
                LinkText = ColorTranslator.FromWin32(unchecked((int)linkText)),

                Edge = ColorTranslator.FromWin32(unchecked((int)edge)),
                HotEdge = ColorTranslator.FromWin32(unchecked((int)hotEdge))
            };
        }
    }
}