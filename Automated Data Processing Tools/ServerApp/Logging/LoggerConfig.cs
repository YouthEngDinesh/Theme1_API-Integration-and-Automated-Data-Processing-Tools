using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace ServerApp.Logging
{
    public static class LoggerConfig
    {
        public static void Configure()
        {
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Information()
                 .WriteTo.File(
                     "Server.log",
                     rollingInterval: RollingInterval.Day)
                 .CreateLogger();
        }
    }

}
