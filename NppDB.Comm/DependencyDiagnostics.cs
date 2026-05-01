using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;

namespace NppDB.Comm
{
    public static class DependencyDiagnostics
    {
        private const string AceProviderProgId = "Microsoft.ACE.OLEDB.12.0";
        private const string AccessEngineDownloadUrl = "https://www.microsoft.com/en-us/download/details.aspx?id=54920";
        private const string DotNet48DownloadUrl = "https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net48-web-installer";

        public static string CurrentProcessArchitectureLabel => Environment.Is64BitProcess ? "64-bit" : "32-bit";

        public static bool IsMsAccessAceProviderAvailableForCurrentProcess()
        {
            return IsAceRegisteredInView(Environment.Is64BitProcess ? RegistryView.Registry64 : RegistryView.Registry32);
        }

        public static bool IsMsAccessAceProviderAvailableInOppositeBitness()
        {
            return IsAceRegisteredInView(Environment.Is64BitProcess ? RegistryView.Registry32 : RegistryView.Registry64);
        }

        public static string BuildMsAccessStartupWarningMessage()
        {
            if (IsMsAccessAceProviderAvailableForCurrentProcess())
            {
                return null;
            }

            return BuildMsAccessProviderMessage(null, null);
        }

        public static string BuildMsAccessConnectionErrorMessage(Exception ex, string databasePath)
        {
            var flattened = FlattenMessages(ex);
            var lowered = flattened.ToLowerInvariant();

            if (!IsMsAccessAceProviderAvailableForCurrentProcess() ||
                ContainsAny(lowered,
                    "provider is not registered",
                    "class not registered",
                    "microsoft.ace.oledb.12.0",
                    "could not find installable isam"))
            {
                return BuildMsAccessProviderMessage(databasePath, flattened);
            }

            if (ContainsAny(lowered,
                    "specified module could not be found",
                    "unable to load dll",
                    "could not load file or assembly"))
            {
                return BuildMsAccessVcRuntimeMessage(databasePath, flattened);
            }

            if (ContainsAny(lowered,
                    "not a valid password",
                    "invalid password",
                    "password"))
            {
                return BuildMessage(
                    "MS Access connection failed",
                    databasePath,
                    "NppDB could not open the Access database with the supplied password.",
                    new[]
                    {
                        "Check: re-enter the database password and make sure this is the expected database file."
                    },
                    flattened);
            }

            if (ContainsAny(lowered, "could not find file", "cannot find the input table or query"))
            {
                return BuildMessage(
                    "MS Access connection failed",
                    databasePath,
                    "NppDB could not find the selected Access database file.",
                    new[]
                    {
                        "Check: confirm that the file still exists and that the path is correct."
                    },
                    flattened);
            }

            if (ContainsAny(lowered, "unrecognized database format"))
            {
                return BuildMessage(
                    "MS Access connection failed",
                    databasePath,
                    "The selected file is not in a format that the installed Access driver can open.",
                    new[]
                    {
                        "Check: make sure the file is a real .accdb/.mdb database and is not corrupted."
                    },
                    flattened);
            }

            if (ContainsAny(lowered,
                    "already in use",
                    "opened exclusively",
                    "file already in use",
                    "could not lock file"))
            {
                return BuildMessage(
                    "MS Access connection failed",
                    databasePath,
                    "The database file is locked by another process or opened in exclusive mode.",
                    new[]
                    {
                        "Check: close Microsoft Access or other tools using the file, then try again."
                    },
                    flattened);
            }

            if (ContainsAny(lowered,
                    "cannot open or write to the file",
                    "access is denied",
                    "permission denied"))
            {
                return BuildMessage(
                    "MS Access connection failed",
                    databasePath,
                    "Windows or the Access driver does not have enough access to open the database file.",
                    new[]
                    {
                        "Check: verify file and folder permissions and make sure the containing folder is writable."
                    },
                    flattened);
            }

            return BuildMessage(
                "MS Access connection failed",
                databasePath,
                "NppDB could not open the Access database.",
                new[]
                {
                    "Check: verify that the Access driver is installed, that its architecture matches Notepad++, and that the selected database file and password are correct."
                },
                flattened);
        }

        public static string BuildPostgreSqlValidationMessage(string host, string port, string database, string username)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return "PostgreSQL connection details are incomplete.\n\nEnter a server/host name such as localhost or the database server address.";
            }

            if (string.IsNullOrWhiteSpace(port))
            {
                return "PostgreSQL connection details are incomplete.\n\nEnter the PostgreSQL port, usually 5432 unless your server uses a different one.";
            }

            if (!int.TryParse(port, out var portNumber) || portNumber < 1 || portNumber > 65535)
            {
                return "PostgreSQL connection details are invalid.\n\nThe port must be a number between 1 and 65535.";
            }

            if (string.IsNullOrWhiteSpace(database))
            {
                return "PostgreSQL connection details are incomplete.\n\nEnter the target database name.";
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                return "PostgreSQL connection details are incomplete.\n\nEnter the PostgreSQL username/role for this connection.";
            }

            return null;
        }

        public static string BuildPostgreSqlConnectionErrorMessage(Exception ex, string host, string port, string database, string username)
        {
            var flattened = FlattenMessages(ex);
            var lowered = flattened.ToLowerInvariant();
            var target = BuildPostgreSqlTarget(host, port, database, username);

            if (ContainsAny(lowered, "password authentication failed", "28p01"))
            {
                return BuildMessage(
                    "PostgreSQL connection failed",
                    target,
                    "The PostgreSQL server rejected the username or password.",
                    new[]
                    {
                        "Check: confirm the username and password."
                    },
                    flattened);
            }

            if (ContainsAny(lowered,
                    "no such host is known",
                    "host not known",
                    "name or service not known",
                    "could not translate host name"))
            {
                return BuildMessage(
                    "PostgreSQL connection failed",
                    target,
                    "The PostgreSQL host name could not be resolved.",
                    new[]
                    {
                        "Check: verify the host name, DNS resolution, VPN requirements, or use an IP address."
                    },
                    flattened);
            }

            if (ContainsAny(lowered,
                    "connection refused",
                    "actively refused",
                    "failed to connect to",
                    "target machine actively refused"))
            {
                return BuildMessage(
                    "PostgreSQL connection failed",
                    target,
                    "The PostgreSQL server is not accepting connections on the specified host/port.",
                    new[]
                    {
                        "Check: confirm the server address, port, firewall rules, and that PostgreSQL is running."
                    },
                    flattened);
            }

            if (ContainsAny(lowered, "timeout", "timed out"))
            {
                return BuildMessage(
                    "PostgreSQL connection failed",
                    target,
                    "The connection attempt timed out before the PostgreSQL server responded.",
                    new[]
                    {
                        "Check: confirm the host/port, firewall/VPN settings, and whether the server is reachable from this machine."
                    },
                    flattened);
            }

            if (ContainsAny(lowered, "does not exist") && lowered.Contains("database"))
            {
                return BuildMessage(
                    "PostgreSQL connection failed",
                    target,
                    "The specified PostgreSQL database does not exist on the target server.",
                    new[]
                    {
                        "Check: verify the database name and ensure it exists on the selected server."
                    },
                    flattened);
            }

            if (ContainsAny(lowered, "does not exist") && lowered.Contains("role"))
            {
                return BuildMessage(
                    "PostgreSQL connection failed",
                    target,
                    "The specified PostgreSQL user/role does not exist on the target server.",
                    new[]
                    {
                        "Check: verify the username and create or grant the role on the server if needed."
                    },
                    flattened);
            }

            return BuildMessage(
                "PostgreSQL connection failed",
                target,
                "NppDB could not establish the PostgreSQL connection.",
                new[]
                {
                    "Check: verify the host, port, database name, username, password, server availability, and firewall/VPN settings."
                },
                flattened);
        }

        public static string BuildPluginAssemblyLoadErrorMessage(string filePath, Exception ex)
        {
            var fileName = Path.GetFileName(filePath) ?? filePath ?? "Unknown file";
            var lowered = FlattenMessages(ex).ToLowerInvariant();

            if (ex is BadImageFormatException)
            {
                return BuildMessage(
                    "NppDB could not load one of its plugin modules",
                    fileName,
                    "The module architecture does not match the running Notepad++ process.",
                    new[]
                    {
                        "Check: use the matching 32-bit/64-bit NppDB package for your Notepad++ installation and make sure copied DLLs were not mixed from different builds."
                    },
                    ex.Message);
            }

            return "NppDB could not start because a required runtime or dependency is missing." + Environment.NewLine + Environment.NewLine + "Download:" + Environment.NewLine + DotNet48DownloadUrl;
        }

        private static string BuildMsAccessProviderMessage(string databasePath, string originalError)
        {
            var oppositeBitnessInstalled = IsMsAccessAceProviderAvailableInOppositeBitness();
            var summary = oppositeBitnessInstalled
                ? $"NppDB is running in {CurrentProcessArchitectureLabel}, but the installed Microsoft Access driver is only available in {GetOppositeArchitectureLabel()}."
                : $"Microsoft Access driver is missing for this {CurrentProcessArchitectureLabel} Notepad++ build.";

            return summary + Environment.NewLine + Environment.NewLine + "Download:" + Environment.NewLine + AccessEngineDownloadUrl;
        }

        private static string BuildMsAccessVcRuntimeMessage(string databasePath, string originalError)
        {
            return $"Microsoft Access driver could not start because a required Visual C++ runtime is missing for this {CurrentProcessArchitectureLabel} build."
                + Environment.NewLine + Environment.NewLine + "Download:" + Environment.NewLine + GetVcRedistDownloadUrl();
        }

        private static bool IsAceRegisteredInView(RegistryView view)
        {
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, view))
                using (var subKey = baseKey.OpenSubKey(AceProviderProgId))
                {
                    return subKey != null;
                }
            }
            catch
            {
                return false;
            }
        }

        private static string FlattenMessages(Exception ex)
        {
            if (ex == null)
            {
                return string.Empty;
            }

            return string.Join(" | ", EnumerateExceptions(ex)
                .Select(x => x.Message)
                .Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private static string BuildMessage(string heading, string target, string summary, string[] guidanceLines, string originalError)
        {
            var parts = new System.Collections.Generic.List<string>
            {
                heading
            };

            if (!string.IsNullOrWhiteSpace(target))
            {
                parts.Add(string.Empty);
                parts.Add("Target: " + target);
            }

            parts.Add(string.Empty);
            parts.Add(summary);

            if (guidanceLines != null)
            {
                foreach (var line in guidanceLines.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    parts.Add(line);
                }
            }


            return string.Join(Environment.NewLine, parts);
        }

        private static string BuildPostgreSqlTarget(string host, string port, string database, string username)
        {
            return string.Format("{0}:{1} / {2} / user {3}",
                string.IsNullOrWhiteSpace(host) ? "(no host)" : host,
                string.IsNullOrWhiteSpace(port) ? "(no port)" : port,
                string.IsNullOrWhiteSpace(database) ? "(no database)" : database,
                string.IsNullOrWhiteSpace(username) ? "(no username)" : username);
        }

        private static string GetOppositeArchitectureLabel()
        {
            return Environment.Is64BitProcess ? "32-bit" : "64-bit";
        }

        private static string GetVcRedistDownloadUrl()
        {
            return Environment.Is64BitProcess
                ? "https://aka.ms/vs/17/release/vc_redist.x64.exe"
                : "https://aka.ms/vs/17/release/vc_redist.x86.exe";
        }

        private static bool ContainsAny(string value, params string[] needles)
        {
            if (string.IsNullOrEmpty(value) || needles == null)
            {
                return false;
            }

            return needles.Any(needle => !string.IsNullOrWhiteSpace(needle) && value.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static System.Collections.Generic.IEnumerable<Exception> EnumerateExceptions(Exception ex)
        {
            var current = ex;
            while (current != null)
            {
                yield return current;
                current = current.InnerException;
            }
        }
    }
}
