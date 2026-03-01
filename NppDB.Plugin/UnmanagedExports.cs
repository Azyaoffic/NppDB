using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Kbg.NppPluginNET.PluginInfrastructure;
using NppPlugin.DllExport;

namespace NppDB
{
    internal static class UnmanagedExports
    {
        private static readonly object _plugin;

        private static readonly MethodInfo _miIsUnicode;
        private static readonly MethodInfo _miSetInfo;
        private static readonly MethodInfo _miGetFuncsArray;
        private static readonly MethodInfo _miMessageProc;
        private static readonly MethodInfo _miGetName;
        private static readonly MethodInfo _miBeNotified;

        static UnmanagedExports()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveFromPluginDir;

            var asm = typeof(UnmanagedExports).Assembly;
            var pluginType = asm.GetType("NppDB.NppDbPlugin", throwOnError: true);

            _plugin = Activator.CreateInstance(pluginType);

            _miIsUnicode = pluginType.GetMethod("IsUnicode", BindingFlags.Public | BindingFlags.Static);
            _miSetInfo = pluginType.GetMethod("SetInfo", BindingFlags.Public | BindingFlags.Instance);
            _miGetFuncsArray = pluginType.GetMethod("GetFuncsArray", BindingFlags.Public | BindingFlags.Static);
            _miMessageProc = pluginType.GetMethod("MessageProc", BindingFlags.Public | BindingFlags.Instance);
            _miGetName = pluginType.GetMethod("GetName", BindingFlags.Public | BindingFlags.Instance);
            _miBeNotified = pluginType.GetMethod("BeNotified", BindingFlags.Public | BindingFlags.Instance);
        }

        private static Assembly ResolveFromPluginDir(object sender, ResolveEventArgs args)
        {
            try
            {
                var requested = new AssemblyName(args.Name).Name + ".dll";
                var pluginDir = Path.GetDirectoryName(typeof(UnmanagedExports).Assembly.Location);

                var p1 = Path.Combine(pluginDir, requested);
                if (File.Exists(p1))
                    return Assembly.LoadFrom(p1);

                var p2 = Path.Combine(pluginDir, "lib", requested);
                if (File.Exists(p2))
                    return Assembly.LoadFrom(p2);
            }
            catch
            {
                // returning null lets default resolution continue
            }

            return null;
        }

        [DllExport(CallingConvention=CallingConvention.Cdecl)]
        static bool isUnicode()
        {
            return (bool)_miIsUnicode.Invoke(null, null);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static void setInfo(NppData notepadPlusData)
        {
            _miSetInfo.Invoke(_plugin, new object[] { notepadPlusData });
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static IntPtr getFuncsArray(ref int nbF)
        {
            var a = new object[] { nbF };
            var ret = (IntPtr)_miGetFuncsArray.Invoke(null, a);
            nbF = (int)a[0];
            return ret;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static uint messageProc(uint message, IntPtr wParam, IntPtr lParam)
        {
            return Convert.ToUInt32(_miMessageProc.Invoke(_plugin, new object[] { message, wParam, lParam }));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static IntPtr getName()
        {
            return (IntPtr)_miGetName.Invoke(_plugin, null);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static void beNotified(IntPtr notifyCode)
        {
            if (notifyCode == IntPtr.Zero)
                return;

            var notification = (ScNotification)Marshal.PtrToStructure(notifyCode, typeof(ScNotification));
            _miBeNotified.Invoke(_plugin, new object[] { notification });
        }
    }
}