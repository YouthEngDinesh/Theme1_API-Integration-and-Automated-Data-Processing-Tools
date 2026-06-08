using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp.Models
{
    // クライアントからのリクエストを表すクラス
    class ClientRequest
    {
        // クライアントからのリクエストの内容を表すプロパティ

        public string Action { get; set; } = string.Empty;
        // クライアントからのリクエストのIDを表すプロパティ
        public int Id { get; set; }
        // クライアントからのリクエストの設備データを表すプロパティ
        public DeviceLog? DeviceLog { get; set; }

    }
}
