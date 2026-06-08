using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using ClientApp.Models;
using Serilog;

namespace ClientApp.Services
{
    // HTTP通信を行うサービスクラス
    public class ApiClientService
    {
        //クライアントがサーバーにリクエストを送信するためのサービスクラス
        private readonly HttpClient _client = new();
         //サーバーからのレスポンスを受け取るための非同期メソッド
        internal async Task<ApiResponse?>
            SendAsync(ClientRequest request)
            {
            //リクエストの内容をログに記録
            Log.Information( "Sending Request: {Action}",request.Action);
            // リクエストデータをJSONへ変換
            string json = JsonSerializer.Serialize(request);

                var content = new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json");
            // サーバーへPOSTリクエスト送信
            var response = await _client.PostAsync( "http://localhost:8080/",content);
            Log.Error("Response Status: {Status}",response.StatusCode);
            //var response1 = await _client.GetAsync( "http://localhost:8080/");
            Log.Information("Response Status: {Status}",response.StatusCode);
            // サーバーからのレスポンスを取得
            string responseJson = await response.Content.ReadAsStringAsync();
            // 通信開始ログ
            Log.Information("Response Content: {Content}", responseJson);
            // JSONレスポンスをオブジェクトへ変換
            return JsonSerializer
                .Deserialize<ApiResponse>(
                    responseJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
        }
    
    }
}
