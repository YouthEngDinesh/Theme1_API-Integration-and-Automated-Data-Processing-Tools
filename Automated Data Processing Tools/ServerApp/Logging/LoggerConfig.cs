using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace ServerApp.Logging;

    public static class LoggerConfig
    {
    public static void Init()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                "logs/app-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();
    }

    public static void Close()
    {
        Log.CloseAndFlush();
    }
}

