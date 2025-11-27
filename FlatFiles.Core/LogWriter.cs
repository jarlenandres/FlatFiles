using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFiles.Core;

public class LogWriter
{

    private static readonly string _logFile = "log.txt";

    public static void Log(string username, string message)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(_logFile)) ?? ".");
        var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | Usuario: {username} | {message}";
        File.AppendAllText(_logFile, line + Environment.NewLine);
    }

    public static void LogSystem(string message)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(_logFile)) ?? ".");
        var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | Sistema | {message}";
        File.AppendAllText(_logFile, line + Environment.NewLine);
    }

}