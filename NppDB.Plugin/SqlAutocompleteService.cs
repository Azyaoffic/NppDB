using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using Npgsql;
using NppDB.Comm;
using NppDB.MSAccess;
using NppDB.PostgreSQL;
using Kbg.NppPluginNET.PluginInfrastructure;

namespace NppDB
{
    internal static class SqlAutocompleteService
    {
        private sealed class CacheEntry
        {
            public DateTime FetchedUtc { get; set; }
            public string AllItems { get; set; }
            public string ColumnItems { get; set; }
        }

        private static readonly HashSet<long> ActivePluginAutocompleteBuffers = new HashSet<long>();
        private static readonly object SyncRoot = new object();
        private static readonly Dictionary<string, CacheEntry> Cache = new Dictionary<string, CacheEntry>(StringComparer.Ordinal);
        private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(2);

        public static void Invalidate(IDbConnect connection)
        {
            if (connection == null) return;

            lock (SyncRoot)
            {
                Cache.Remove(BuildCacheKey(connection));
                ActivePluginAutocompleteBuffers.Clear();
            }
        }
        
        private static long BufferKey(IntPtr bufferId)
        {
            return bufferId.ToInt64();
        }

        private static bool IsPluginAutocompleteActive(IntPtr bufferId)
        {
            lock (SyncRoot)
            {
                return ActivePluginAutocompleteBuffers.Contains(BufferKey(bufferId));
            }
        }

        private static void SetPluginAutocompleteActive(IntPtr bufferId, bool active)
        {
            lock (SyncRoot)
            {
                var key = BufferKey(bufferId);
                if (active)
                {
                    ActivePluginAutocompleteBuffers.Add(key);
                }
                else
                {
                    ActivePluginAutocompleteBuffers.Remove(key);
                }
            }
        }

        public static void TryShowForTypedChar(IDbConnect connection, IScintillaGateway editor, IntPtr bufferId, int typedChar)
        {
            if (connection == null || editor == null || !connection.IsOpened)
            {
                return;
            }

            ConfigureEditor(editor);
            RefreshOrCancel(connection, editor, bufferId);
        }
        
        public static void RefreshOrCancel(IDbConnect connection, IScintillaGateway editor, IntPtr bufferId)
        {
            if (connection == null || editor == null || !connection.IsOpened)
            {
                CancelIfOwned(editor, bufferId);
                return;
            }

            var currentPos = editor.GetCurrentPos();
            if (currentPos < 0)
            {
                CancelIfOwned(editor, bufferId);
                return;
            }

            var cacheEntry = GetOrCreate(connection);
            var prefix = GetCompletionPrefix(editor, currentPos);
            var prefixStart = currentPos - prefix.Length;

            var isAfterDot = prefixStart > 0 && Convert.ToChar(editor.GetCharAt(prefixStart - 1)) == '.';

            if (isAfterDot)
            {
                var matchingColumns = FilterItems(cacheEntry.ColumnItems, prefix, true);
                if (!string.IsNullOrWhiteSpace(matchingColumns))
                {
                    ShowAutocomplete(editor, bufferId, prefix.Length, matchingColumns);
                    return;
                }

                CancelIfOwned(editor, bufferId);
                return;
            }

            if (prefix.Length >= 2)
            {
                var matchingItems = FilterItems(cacheEntry.AllItems, prefix, false);
                if (!string.IsNullOrWhiteSpace(matchingItems))
                {
                    ShowAutocomplete(editor, bufferId, prefix.Length, matchingItems);
                    return;
                }
            }

            CancelIfOwned(editor, bufferId);
        }
        
        private static string FilterItems(string itemList, string prefix, bool allowEmptyPrefix)
        {
            if (string.IsNullOrWhiteSpace(itemList))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(prefix))
            {
                if (!allowEmptyPrefix)
                {
                    return string.Empty;
                }

                return string.Join("\n",
                    itemList
                        .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Take(200));
            }

            return string.Join("\n",
                itemList
                    .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => x.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    .Take(200));
        }

        private static void ShowAutocomplete(IScintillaGateway editor, IntPtr bufferId, int lengthEntered, string itemList)
        {
            if (string.IsNullOrWhiteSpace(itemList))
            {
                CancelIfOwned(editor, bufferId);
                return;
            }

            if (editor.AutoCActive())
            {
                editor.AutoCCancel();
            }

            editor.AutoCShow(lengthEntered, itemList);
            SetPluginAutocompleteActive(bufferId, true);
        }
        
        private static void CancelIfOwned(IScintillaGateway editor, IntPtr bufferId)
        {
            if (!IsPluginAutocompleteActive(bufferId))
            {
                return;
            }

            if (editor != null && editor.AutoCActive())
            {
                editor.AutoCCancel();
            }

            SetPluginAutocompleteActive(bufferId, false);
        }

        private static void ConfigureEditor(IScintillaGateway editor)
        {
            editor.AutoCSetSeparator('\n');
            editor.AutoCSetIgnoreCase(true);
            editor.AutoCSetCancelAtStart(true);
            editor.AutoCSetAutoHide(true);
            editor.AutoCSetDropRestOfWord(false);
            editor.AutoCSetChooseSingle(false);
            editor.AutoCSetOrder(Ordering.PRESORTED);
            editor.AutoCStops(" \t\r\n(){}[],:;=+-*/%<>!&|^~?@.");
        }

        private static CacheEntry GetOrCreate(IDbConnect connection)
        {
            var cacheKey = BuildCacheKey(connection);

            lock (SyncRoot)
            {
                CacheEntry existing;
                if (Cache.TryGetValue(cacheKey, out existing) && DateTime.UtcNow - existing.FetchedUtc < CacheLifetime)
                {
                    return existing;
                }
            }

            var created = BuildCacheEntry(connection);

            lock (SyncRoot)
            {
                Cache[cacheKey] = created;
            }

            return created;
        }

        private static CacheEntry BuildCacheEntry(IDbConnect connection)
        {
            if (connection is PostgreSqlConnect)
            {
                return BuildPostgreSqlCache((PostgreSqlConnect)connection);
            }

            if (connection is MsAccessConnect)
            {
                return BuildMsAccessCache((MsAccessConnect)connection);
            }

            return EmptyCacheEntry();
        }

        private static CacheEntry BuildPostgreSqlCache(PostgreSqlConnect connection)
        {
            var allItems = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            var columnItems = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

            var port = 5432;
            int.TryParse(connection.Port, out port);

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = connection.ServerAddress,
                Port = port,
                Database = connection.Database,
                Username = connection.Account,
                Password = connection.Password,
                IncludeErrorDetail = true,
            };

            using (var cnn = new NpgsqlConnection(builder.ConnectionString))
            {
                cnn.Open();

                const string tableQuery = @"
SELECT table_schema, table_name
FROM information_schema.tables
WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
  AND table_type IN ('BASE TABLE', 'VIEW', 'FOREIGN TABLE')
ORDER BY table_schema, table_name;";

                using (var cmd = new NpgsqlCommand(tableQuery, cnn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var schemaName = reader["table_schema"].ToString();
                        var tableName = reader["table_name"].ToString();

                        AddPostgreSqlIdentifier(allItems, tableName);
                        AddPostgreSqlQualifiedTable(allItems, schemaName, tableName);
                    }
                }

                const string columnQuery = @"
SELECT table_schema, table_name, column_name
FROM information_schema.columns
WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
ORDER BY table_schema, table_name, ordinal_position;";

                using (var cmd = new NpgsqlCommand(columnQuery, cnn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var columnName = reader["column_name"].ToString();
                        AddPostgreSqlIdentifier(allItems, columnName);
                        AddPostgreSqlIdentifier(columnItems, columnName);
                    }
                }
            }

            return CreateCacheEntry(allItems, columnItems);
        }

        private static CacheEntry BuildMsAccessCache(MsAccessConnect connection)
        {
            var allItems = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            var columnItems = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var cnn = new OleDbConnection(BuildMsAccessConnectionString(connection)))
            {
                cnn.Open();

                using (var tables = cnn.GetSchema(OleDbMetaDataCollectionNames.Tables))
                {
                    foreach (DataRow row in tables.Rows)
                    {
                        var tableType = row["TABLE_TYPE"] as string;
                        var tableName = row["TABLE_NAME"] as string;

                        if (string.IsNullOrWhiteSpace(tableName) || IsSystemAccessObject(tableName))
                        {
                            continue;
                        }

                        if (!string.Equals(tableType, "TABLE", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(tableType, "VIEW", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        AddMsAccessIdentifier(allItems, tableName);
                    }
                }

                using (var columns = cnn.GetSchema(OleDbMetaDataCollectionNames.Columns))
                {
                    foreach (DataRow row in columns.Rows)
                    {
                        var tableName = row["TABLE_NAME"] as string;
                        var columnName = row["COLUMN_NAME"] as string;

                        if (string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(columnName) || IsSystemAccessObject(tableName))
                        {
                            continue;
                        }

                        AddMsAccessIdentifier(allItems, columnName);
                        AddMsAccessIdentifier(columnItems, columnName);
                    }
                }
            }

            return CreateCacheEntry(allItems, columnItems);
        }

        private static CacheEntry CreateCacheEntry(IEnumerable<string> allItems, IEnumerable<string> columnItems)
        {
            return new CacheEntry
            {
                FetchedUtc = DateTime.UtcNow,
                AllItems = string.Join("\n", allItems),
                ColumnItems = string.Join("\n", columnItems),
            };
        }

        private static CacheEntry EmptyCacheEntry()
        {
            return new CacheEntry
            {
                FetchedUtc = DateTime.UtcNow,
                AllItems = string.Empty,
                ColumnItems = string.Empty,
            };
        }

        private static string BuildMsAccessConnectionString(MsAccessConnect connection)
        {
            var builder = new OleDbConnectionStringBuilder
            {
                Provider = "Microsoft.ACE.OLEDB.12.0",
                DataSource = connection.ServerAddress,
                OleDbServices = -4
            };

            if (!string.IsNullOrEmpty(connection.Password))
            {
                builder.Add("Jet OLEDB:Database Password", connection.Password);
            }

            return builder.ConnectionString;
        }


        private static string BuildCacheKey(IDbConnect connection)
        {
            if (connection is PostgreSqlConnect)
            {
                var pg = (PostgreSqlConnect)connection;
                return string.Join("|", "pg", pg.ServerAddress ?? string.Empty, pg.Port ?? string.Empty,
                    pg.Database ?? string.Empty, pg.Account ?? string.Empty);
            }

            if (connection is MsAccessConnect)
            {
                var access = (MsAccessConnect)connection;
                return string.Join("|", "access", access.ServerAddress ?? string.Empty);
            }

            return string.Join("|", connection.Dialect.ToString(), connection.Title ?? string.Empty);
        }

        private static void AddPostgreSqlIdentifier(ISet<string> target, string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return;
            }

            if (RequiresQuotingForPostgreSql(identifier))
            {
                target.Add(QuotePostgreSql(identifier));
                return;
            }

            target.Add(identifier);
        }

        private static void AddPostgreSqlQualifiedTable(ISet<string> target, string schemaName, string tableName)
        {
            if (string.IsNullOrWhiteSpace(schemaName) || string.IsNullOrWhiteSpace(tableName))
            {
                return;
            }

            var left = RequiresQuotingForPostgreSql(schemaName) ? QuotePostgreSql(schemaName) : schemaName;
            var right = RequiresQuotingForPostgreSql(tableName) ? QuotePostgreSql(tableName) : tableName;
            target.Add(left + "." + right);
        }

        private static void AddMsAccessIdentifier(ISet<string> target, string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return;
            }

            if (IsSimpleIdentifier(identifier))
            {
                target.Add(identifier);
            }

            target.Add("[" + identifier.Replace("]", "]]") + "]");
        }

        private static bool IsSystemAccessObject(string name)
        {
            return name.StartsWith("MSys", StringComparison.OrdinalIgnoreCase)
                   || name.StartsWith("USys", StringComparison.OrdinalIgnoreCase);
        }

        private static bool RequiresQuotingForPostgreSql(string identifier)
        {
            return !IsSimpleIdentifier(identifier) || !string.Equals(identifier, identifier.ToLowerInvariant(), StringComparison.Ordinal);
        }

        private static bool IsSimpleIdentifier(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                return false;
            }

            if (!(char.IsLetter(identifier[0]) || identifier[0] == '_'))
            {
                return false;
            }

            for (var i = 1; i < identifier.Length; i++)
            {
                var c = identifier[i];
                if (!(char.IsLetterOrDigit(c) || c == '_' || c == '$'))
                {
                    return false;
                }
            }

            return true;
        }

        private static string QuotePostgreSql(string identifier)
        {
            return "\"" + (identifier ?? string.Empty).Replace("\"", "\"\"") + "\"";
        }

        private static bool IsCompletionChar(char value)
        {
            return char.IsLetterOrDigit(value) || value == '_' || value == '$' || value == '"' || value == '[';
        }

        private static string GetCompletionPrefix(IScintillaGateway editor, int currentPos)
        {
            var chars = new List<char>();

            for (var pos = currentPos - 1; pos >= 0; pos--)
            {
                var ch = Convert.ToChar(editor.GetCharAt(pos));
                if (!IsCompletionChar(ch))
                {
                    break;
                }

                chars.Add(ch);
            }

            chars.Reverse();
            return new string(chars.ToArray());
        }
    }
}