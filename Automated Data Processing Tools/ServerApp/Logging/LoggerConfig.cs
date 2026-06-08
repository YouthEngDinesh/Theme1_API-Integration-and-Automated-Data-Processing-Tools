using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace ServerApp.Logging
{
    // Serilogの設定を行うクラス
    public static class LoggerConfig
    {
        public static void Configure()
        {
            // ログ出力設定の初期化
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Information()
                 .WriteTo.File(
                     "Server.log",
                     rollingInterval: RollingInterval.Day)
                 .CreateLogger();
        }
    }

}
