using System;
using System.Runtime.InteropServices;
using Kbg.NppPluginNET.PluginInfrastructure;
using NppDB.Core;

namespace NppDB
{
    internal static class UiThemeSync
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct NppDarkModeColors
        {
            public uint background;
            public uint softerBackground;
            public uint hotBackground;
            public uint pureBackground;
            public uint errorBackground;

            public uint text;
            public uint darkerText;
            public uint disabledText;
            public uint linkText;

            public uint edge;
            public uint hotEdge;
        }

        private static IntPtr _nppHandle;

        public static void Initialize(IntPtr nppHandle)
        {
            _nppHandle = nppHandle;
            RefreshAndApply();
        }

        public static void RefreshAndApply()
        {
            if (_nppHandle == IntPtr.Zero) return;

            var settings = NppDbSettingsStore.Get();
            var themeMode = (settings?.Behavior?.ThemeMode ?? "FollowNotepadPlusPlus").Trim();
            if (string.IsNullOrWhiteSpace(themeMode)) themeMode = "FollowNotepadPlusPlus";

            var nppIsDark = Win32.SendMessage(_nppHandle, (uint)NppMsg.NPPM_ISDARKMODEENABLED, 0, 0) != IntPtr.Zero;

            if (themeMode == "ForceLight" || (themeMode == "FollowNotepadPlusPlus" && !nppIsDark))
            {
                UiThemeManager.SetPalette(UiThemePalette.CreateLightDefault());
                return;
            }

            if (TryGetDarkColors(out var c))
            {
                UiThemeManager.SetPalette(UiThemePalette.FromNppDarkModeColors(
                    c.background, c.softerBackground, c.hotBackground,
                    c.pureBackground, c.errorBackground,
                    c.text, c.darkerText, c.disabledText, c.linkText,
                    c.edge, c.hotEdge));
            }
            else
            {
                UiThemeManager.SetPalette(UiThemePalette.CreateDarkFallback());
            }
        }

        private static bool TryGetDarkColors(out NppDarkModeColors colors)
        {
            colors = default;

            var size = Marshal.SizeOf(typeof(NppDarkModeColors));
            var ptr = Marshal.AllocHGlobal(size);

            try
            {
                Marshal.StructureToPtr(colors, ptr, false);

                var ok = Win32.SendMessage(_nppHandle, (uint)NppMsg.NPPM_GETDARKMODECOLORS, size, ptr) != IntPtr.Zero;
                if (!ok) return false;

                colors = Marshal.PtrToStructure<NppDarkModeColors>(ptr);
                return true;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}