using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using ClientApp.Models;
using Serilog;

namespace ClientApp.Services
{
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
            //リクエストオブジェクトをJSON形式にシリアライズ
            string json = JsonSerializer.Serialize(request);

                var content = new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json");
            //サーバーにPOSTリクエストを送信し、レスポンスを受け取る
            var response = await _client.PostAsync( "http://localhost:8080/",content);
            Log.Information("Response Status: {Status}",response.StatusCode);
            //レスポンスの内容を文字列として読み取る
            string responseJson = await response.Content.ReadAsStringAsync();
            //レスポンスの内容をログに記録
            Log.Information("Response Content: {Content}", responseJson);
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
