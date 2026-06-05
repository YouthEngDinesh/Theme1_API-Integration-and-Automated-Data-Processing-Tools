using System;
using System.Collections.Generic;
using System.Text;
using Serilog;  


namespace ClientApp.Logging
{
   public static class LoggerConfig
    {
        public static void Configure()
        {
            Log.Logger =
             new LoggerConfiguration()
                 .MinimumLevel.Information()
                 .WriteTo.File(
                     "client.log",
                     rollingInterval: RollingInterval.Day)
                 .CreateLogger();
        }
    }
}
