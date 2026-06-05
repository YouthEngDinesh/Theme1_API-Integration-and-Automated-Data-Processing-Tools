using Dapper;
using Microsoft.SqlServer;
using Serilog;
using ServerApp.Logging;
using ServerApp.Models;
using ServerApp.Repositories;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;


class Program
{
    static async Task Main(string[] args)
    {

        string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=Task1DB;Trusted_Connection=True;";
        var repo = new DeviceLogRepository(connectionString);
        LoggerConfig.Init();


        Log.Information("アプリ開始");


        // 1. サーバーの待ち受け設定（すべてのIPアドレスからの接続をポート 8080 で許可）
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://+:8080/");

        try
        {
            listener.Start();
            throw new Exception("テスト例外");
            
        }
        catch (Exception ex)
        {
            // 管理者権限のエラーが出た場合は、localhostのみで試行します
            //listener.Prefixes.Clear();

            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();
            Log.Error(ex, "エラー発生");
        }

        Console.WriteLine("=========================================");
        Console.WriteLine("【模擬・機器1スタブ】が起動しました");
        Console.WriteLine("   ポート番号: 8080");
        Console.WriteLine("=========================================");
        Console.WriteLine("ペアのPC（クライアント）からの通信を待っています...\n");

        while (true)
        {
            // 2. クライアントからの指示（POST）を待つ
            var context = await listener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;

            object responseObj;

            if (request.HttpMethod == "POST")
            {
                // 3. 送られてきた指示（JSON）を読み取る
                using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                string jsonString = await reader.ReadToEndAsync();

                //判別するための処理
                var req = JsonSerializer.Deserialize<ClientRequest>(jsonString);

                if (req != null)
                {
                    responseObj = new { Success = false, Message = "Invalid JSON" };

                }
                else
                {
                    try
                    {
                        switch (req.Action)
                        {
                            case "Add":
                                await repo.Add(req.DeviceLog);
                                responseObj = new { Success = true };
                                break;
                            case "GetAll":
                                var data = await repo.GetAll();

                                responseObj = new ApiResponse
                                {
                                    Success = true,
                                    Data = data
                                };
                                break;
                            case "Update":
                                await repo.Update(req.DeviceLog);
                                responseObj = new { Success = true };
                                break;

                            case "Delete":
                                await repo.Delete(req.Id);
                                responseObj = new { Success = true };
                                break;

                            default:
                                responseObj = new { Success = false, Message = "Invalid Action" };
                                break;



                        }
                    }
                    catch (Exception ex) 
                    {
                        Log.Error(ex, "Request processing faild");

                        responseObj = new
                        {
                            Success = true,
                            Message = "Server Error"
                        };
                    }
                }







                Console.WriteLine($"[指示受信] 時刻: {DateTime.Now:HH:mm:ss}");
                Console.WriteLine($"[受信データ] {jsonString}");

                // 4. 機器が動作したと見立てて、返事（JSONデータ）を 200 OK で返す
                string jsonResponse = JsonSerializer.Serialize(responseObj);
                byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);

                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.OK; // 200 OK
                response.ContentLength64 = buffer.Length;

                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();

                Console.WriteLine("クライアントへ応答データを返却しました。\n");
            }
            else if(request.HttpMethod == "GET")
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                response.Close();
            }
        }
    }
}