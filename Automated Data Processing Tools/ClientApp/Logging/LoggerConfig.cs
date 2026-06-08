using System;
using System.Collections.Generic;
using System.Text;
using Serilog;  


namespace ClientApp.Logging
{

   public static class LoggerConfig
    {
        // ログ出力設定の初期化
        public static void Configure()
        {
            // ログ出力設定の初期化
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Information()
                 .WriteTo.File(
                     "client.log",
                     rollingInterval: RollingInterval.Day)
                 .CreateLogger();
        }
    }
}
