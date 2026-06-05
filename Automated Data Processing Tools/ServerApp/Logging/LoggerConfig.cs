using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace ServerApp.Logging;

    public static class LoggerConfig
    {
        private const string LogFile = "server.log";

        public static void Info(string msg)
        {
            Write("INFO", msg);
        }

        public static void Warn(string msg)
        {
            Write("WARN", msg);
        }

        public static void Error(string msg)
        {
            Write("ERROR", msg);
        }

        private static void Write(string level, string msg)
        {
            string line =
                $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {msg}";

            File.AppendAllText(
                LogFile,
                line + Environment.NewLine);
        }
    }

