using System;
using System.Collections.Generic;
using System.Text;

namespace ClientApp.Models
{
    // デバイスのログを表すクラス
    class DeviceLog
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }

        public decimal Amount { get; set; }

        public string ErrorCode { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
