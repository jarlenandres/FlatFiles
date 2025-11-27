using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFiles.Core;

public class LogWriter : IDisposable
{
    private readonly StreamWriter _writer;
    private readonly object _sync = new();
    private bool _disposed;

    public LogWriter(string path)
    {
        _writer = new StreamWriter(path, append: true)
        {
            AutoFlush = true
        };
    }

    public void WriteLog(string level, string user, string messege)
    {
        var timestamp = DateTime.Now.ToString("s");
        _writer.WriteLine($"[{timestamp}] - [{level}] - [{user}]- {messege}");
    }

    public void Append<T>(T record)
    {
        if (record is null) throw new ArgumentNullException(nameof(record));
        ThrowIfDisposed();

        lock (_sync)
        {
            // Make sure the underlying stream is positioned at the end.
            _writer.Flush();
            if (_writer.BaseStream.CanSeek)
            {
                _writer.BaseStream.Seek(0, SeekOrigin.End);
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };

            using var csv = new CsvWriter(_writer, config);
            csv.WriteRecord(record);
            csv.NextRecord();
            _writer.Flush();
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(LogWriter));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _writer.Dispose();
        _disposed = true;
    }
}