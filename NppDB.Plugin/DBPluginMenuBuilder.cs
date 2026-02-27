using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Kbg.NppPluginNET.PluginInfrastructure;

namespace NppDB
{
    internal sealed class DbPluginMenuBuilder
    {
        private readonly string _pluginName;
        private bool _rebuilt;

        public DbPluginMenuBuilder(string pluginName)
        {
            _pluginName = pluginName;
        }

        public void TryRebuildOnce(IntPtr nppHandle, FuncItems funcItems)
        {
            if (_rebuilt) return;
            _rebuilt = true;

            try
            {
                funcItems.RefreshItems();

                var pluginsMenu = Win32.SendMessage(
                    nppHandle,
                    (uint)NppMsg.NPPM_GETMENUHANDLE,
                    (int)NppMsg.NPPPLUGINMENU,
                    0);

                if (pluginsMenu == IntPtr.Zero) return;

                var nppDbSubMenu = FindSubMenuByCaption(pluginsMenu, _pluginName);
                if (nppDbSubMenu == IntPtr.Zero) return;

                var existingTextByCmdId = ReadMenuTextByCmdId(nppDbSubMenu);
                if (existingTextByCmdId.Count == 0) return;

                var cmdIdByName = funcItems.Items.ToDictionary(x => x._itemName, x => x._cmdID);

                var plan = BuildPlan(cmdIdByName, existingTextByCmdId);
                if (plan.Count == 0) return;

                ClearMenu(nppDbSubMenu);
                InsertPlan(nppDbSubMenu, plan, existingTextByCmdId);

                DrawMenuBar(nppHandle);
            }
            catch
            {
                // not worth crashing
            }
        }

        private static List<int?> BuildPlan(
            Dictionary<string, int> cmdIdByName,
            Dictionary<int, string> existingTextByCmdId)
        {
            // names MUST match SetCommand names
            var groups = new List<List<string>>
            {
                new List<string> {
                    "Execute SQL",
                    "Analyze SQL",
                    "Analyze and Create Prompt",
                    "Analyze and Create Prompt (Issue at Caret)",
                    "Clear analysis",
                },
                new List<string> {
                    "Database Connect Manager",
                    "Show Prompt Library",
                },
                new List<string> {
                    "Settings"
                },
                new List<string> {
                    "Show Tutorial",
                    "Open console",
                    "About",
                }
            };

            var plan = new List<int?>();
            var used = new HashSet<int>();

            foreach (var group in groups)
            {
                var ids = new List<int>();
                foreach (var name in group)
                {
                    if (!cmdIdByName.TryGetValue(name, out var id)) continue;
                    if (!existingTextByCmdId.ContainsKey(id)) continue;
                    ids.Add(id);
                }

                if (ids.Count == 0) continue;

                if (plan.Count > 0) plan.Add(null); // separator
                foreach (var id in ids)
                {
                    plan.Add(id);
                    used.Add(id);
                }
            }

            var leftovers = existingTextByCmdId.Keys
                .Where(id => !used.Contains(id))
                .ToList();

            if (leftovers.Count > 0)
            {
                if (plan.Count > 0) plan.Add(null);
                foreach (var id in leftovers) plan.Add(id);
            }

            return plan;
        }

        private static void ClearMenu(IntPtr hMenu)
        {
            var count = GetMenuItemCount(hMenu);
            for (var i = count - 1; i >= 0; i--)
                RemoveMenu(hMenu, (uint)i, MF_BYPOSITION);
        }

        private static void InsertPlan(IntPtr hMenu, List<int?> plan, Dictionary<int, string> textByCmdId)
        {
            uint pos = 0;

            foreach (var entry in plan)
            {
                if (entry == null)
                {
                    InsertMenu(hMenu, pos++, MF_BYPOSITION | MF_SEPARATOR, UIntPtr.Zero, null);
                    continue;
                }

                var cmdId = entry.Value;
                if (!textByCmdId.TryGetValue(cmdId, out var text)) continue;

                InsertMenu(hMenu, pos++, MF_BYPOSITION | MF_STRING, (UIntPtr)(uint)cmdId, text);
            }
        }

        private static IntPtr FindSubMenuByCaption(IntPtr parentMenu, string caption)
        {
            var count = GetMenuItemCount(parentMenu);
            for (var i = 0; i < count; i++)
            {
                var sub = GetSubMenu(parentMenu, i);
                if (sub == IntPtr.Zero) continue;

                var sb = new StringBuilder(256);
                GetMenuString(parentMenu, (uint)i, sb, sb.Capacity, MF_BYPOSITION);

                var text = sb.ToString().Replace("&", "").Trim();
                if (string.Equals(text, caption, StringComparison.OrdinalIgnoreCase))
                    return sub;
            }

            return IntPtr.Zero;
        }

        private static Dictionary<int, string> ReadMenuTextByCmdId(IntPtr menu)
        {
            var map = new Dictionary<int, string>();
            var count = GetMenuItemCount(menu);

            for (var i = 0; i < count; i++)
            {
                var id = GetMenuItemID(menu, i);
                if (id <= 0) continue; // -1 separator, 0 invalid

                var sb = new StringBuilder(512);
                GetMenuString(menu, (uint)id, sb, sb.Capacity, MF_BYCOMMAND);
                var text = sb.ToString();

                if (!string.IsNullOrEmpty(text))
                    map[id] = text;
            }

            return map;
        }

        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_BYPOSITION = 0x00000400;
        private const uint MF_SEPARATOR = 0x00000800;
        private const uint MF_STRING = 0x00000000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetMenuItemID(IntPtr hMenu, int nPos);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetMenuString(IntPtr hMenu, uint uIDItem, StringBuilder lpString, int nMaxCount, uint uFlag);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool InsertMenu(IntPtr hMenu, uint uPosition, uint uFlags, UIntPtr uIDNewItem, string lpNewItem);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DrawMenuBar(IntPtr hWnd);
    }
}