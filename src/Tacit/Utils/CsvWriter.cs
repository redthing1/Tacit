using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tacit.Utils;

public class CsvWriter {
    private readonly StreamWriter _sw;
    private Stream _outStream;

    public CsvWriter(Stream outStream) {
        _outStream = outStream;
        _sw = new StreamWriter(outStream);
    }

    public void Header(params string[] columns) {
        var sb = new StringBuilder();
        for (var i = 0; i < columns.Length; i++) {
            sb.Append(columns[i]);
            sb.Append(",");
        }

        _sw.WriteLine(sb.ToString());
        _sw.Flush();
    }

    public void Data(IEnumerable<string[]> data) {
        foreach (var row in data) {
            foreach (var point in row) {
                _sw.Write(point + ",");
            }
            _sw.WriteLine();
        }

        _sw.Flush();
    }
}