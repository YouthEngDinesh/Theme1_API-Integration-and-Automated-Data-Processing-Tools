using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp.Models
{
    //  APIのレスポンスを表すクラス
  class ApiResponse
    {

        // APIリクエストの成功・失敗を示すプロパティ
        public bool Success { get; set; }

        
        // APIリクエストの結果メッセージを示すプロパティ 
        public string Message { get; set; } = string.Empty;

        // APIリクエストの結果データを示すプロパティ
        public object? Data { get; set; }

    }
}
