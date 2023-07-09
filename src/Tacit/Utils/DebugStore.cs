using System.Collections.Generic;

namespace Tacit.Utils;

public class DebugStore {
    public static Dictionary<string, Table> tables = new();

    public static void Point(string table, string[] vals) {
        if (!tables.ContainsKey(table)) tables[table] = new Table();
        tables[table].Add(vals);
    }

    public class Table {
        public List<string[]> data = new();

        public void Add(string[] vals) {
            data.Add(vals);
        }
    }
}