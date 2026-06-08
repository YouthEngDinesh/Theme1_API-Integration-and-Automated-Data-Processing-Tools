using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp.Models
{
    // デバイスのログを表すクラス
    public class DeviceLog
    {
        
        public int Id { get; set; }

        
        public string DeviceName { get; set; } = string.Empty;
        
        
        public decimal Amount { get; set; }


        public string ErrorCode { get; set; } = string.Empty;
        
        
        public DateTime Timestamp { get; set; }

    }
}
