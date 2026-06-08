using System;
using System.Collections.Generic;
using System.Text;

namespace ClientApp.Models
{
    // クライアントからサーバーへのリクエストを表すクラス
    class ClientRequest
    {
        public string Action { get; set; }

        public int Id { get; set; }

        public DeviceLog DeviceLog { get; set; }
    }
}
