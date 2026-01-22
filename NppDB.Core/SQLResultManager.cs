using System;
using System.Collections.Generic;
using System.Linq;
using NppDB.Comm;

namespace NppDB.Core
{
    public class SQLResultManager
    {
        private Dictionary<IntPtr, SqlResult> _bind = new Dictionary<IntPtr, SqlResult>();
        public SqlResult CreateSQLResult(IntPtr id, IDbConnect connect, ISqlExecutor sqlExecutor)
        {
            if (_bind.TryGetValue(id, out var existing))
            {
                if (existing != null && !existing.IsDisposed)
                {
                    return existing;
                }
                _bind.Remove(id);
            }

            var created = new SqlResult(connect, sqlExecutor) { Visible = false }; // Visible=false prevents flicker
            _bind[id] = created;
            return created;
        }

        public int Count => _bind.Count;
        
        public void Remove(IntPtr id)
        {
            _bind.Remove(id);
        }
        public SqlResult GetSQLResult(IntPtr id)
        {
            return _bind.ContainsKey(id) ? _bind[id] : null;
        }
        public void RemoveSQLResults(IDbConnect connect)
        {
            foreach (var result in _bind.Where(x => x.Value.LinkedDbConnect == connect).Select(x => x.Key).ToList())
            {
                _bind.Remove(result);
            }
        }

        private static SQLResultManager _inst;
        public static SQLResultManager Instance
        {
            get
            {
                if (_inst == null) _inst = new SQLResultManager();
                return _inst;
            }
        }
    }
}
