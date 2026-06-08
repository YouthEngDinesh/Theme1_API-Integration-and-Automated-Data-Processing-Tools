using System;
using System.Collections.Generic;
using System.Text;

namespace ClientApp.Models
{
    // サーバーからのレスポンスを表すクラス
    internal class ApiResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }
    }
}
