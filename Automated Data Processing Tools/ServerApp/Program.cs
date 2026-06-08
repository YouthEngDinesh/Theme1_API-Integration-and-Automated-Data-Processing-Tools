using ServerApp.Models;
using ServerApp.Repositories;
using ServerApp.Logging;
using Serilog;

//HTTP
using System.Net;
using System.Text;
using System.Text.Json;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            LoggerConfig.Configure();  // Serilogの設定を行う_Configure the Serilog
            // データベースアクセス用リポジトリ生成
            var repository = new DeviceLogRepository();
            // HTTPサーバーの待ち受け設定
            HttpListener listener = new HttpListener();
            // localhost:8080 でクライアントからの接続を待機
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();
            Console.WriteLine("=================================================");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" ServerApp 開始しました");
            Console.WriteLine("=================================================");
            Console.WriteLine($" Port      : 8080");
            Console.WriteLine($" StartedAt : {DateTime.Now}");
            Console.WriteLine(" Status    :  クライアントからのリクエストを待機中...");
            Console.ResetColor();
            Console.WriteLine("=================================================");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("次のリクエストをお待ちしています...");
                Console.WriteLine("----------------------------------------------");

                HttpListenerContext context = listener.GetContext();
                ProcessRequest(context, repository);

            }
        }

        static void ProcessRequest(HttpListenerContext context,DeviceLogRepository repository)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            if (request.HttpMethod != "POST")
            {
                Console.WriteLine();
                Console.WriteLine("=================================================");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[REQUEST RECEIVED]");
                Console.WriteLine($"Time      : {DateTime.Now:HH:mm:ss}");
                Console.WriteLine($"Method    : {request.HttpMethod}");
                Console.WriteLine($"Client IP : {context.Request.RemoteEndPoint}");
                Console.ResetColor();
                Console.WriteLine("=================================================");
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                response.Close();
                Log.Warning(
                    "Invalid HTTP Method: {Method}",
                    request.HttpMethod);
                return;
            }

            try
            {
                using var reader = new StreamReader(request.InputStream);
                string json = reader.ReadToEnd();
                Console.WriteLine();
                Console.WriteLine("[REQUEST BODY]");
                Console.WriteLine(json);
                Console.WriteLine();

                Log.Information("Request Received: {Json}", json);
                var requestObject = JsonSerializer.Deserialize<ClientRequest>(json);

                Console.WriteLine("[PROCESSING]");
                Console.WriteLine($"Action : {requestObject.Action}");
                Console.WriteLine();

                ApiResponse apiResponse = HandleAction(requestObject, repository);


                string responseJson = JsonSerializer.Serialize(apiResponse);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[RESPONSE JSON]");
               
                Console.WriteLine(responseJson);
                Console.WriteLine();
                byte[] buffer = Encoding.UTF8.GetBytes(responseJson);
                response.ContentType ="application/json";
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer,  0, buffer.Length);
                Console.WriteLine("[SUCCESS]");
                Console.WriteLine("Response sent to client.");
                Console.WriteLine($"Completed At : {DateTime.Now:HH:mm:ss}");
                Console.ResetColor();
                Console.WriteLine("=================================================");
                Console.WriteLine();
                response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Server Error");
                response.StatusCode =(int)HttpStatusCode.InternalServerError;
                response.Close();
            }
        }

        static ApiResponse HandleAction( ClientRequest request,DeviceLogRepository repository)
        {
            switch (request.Action)
            {
                case "Add":

                    Console.WriteLine("[DATABASE]");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("新しいレコードを追加する...");
                    repository.Add(request.DeviceLog);

                    Console.WriteLine("挿入が完了しました。.");
                    Console.ResetColor();
                    return new ApiResponse
                    {
                        Success = true,
                        Message = "Added"
                    };

                case "GetAll":

                    Console.WriteLine("[DATABASE]");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("すべてのレコードを取得する...");

                    var records = repository.GetAll();
                    Console.WriteLine($"レコード数 : {records.Count}");
                    Console.ResetColor();
                    return new ApiResponse
                    {
                        Success = true,
                        Message = "Records Returned",
                        Data = repository.GetAll()
                    };

                case "Update":
                    Console.WriteLine("[DATABASE]");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("レコードの更新中...");

                    repository.Update(request.DeviceLog);
                    
                    Console.WriteLine("更新が完了しました");
                    Console.ResetColor();

                    return new ApiResponse
                    {
                        Success = true,
                        Message = "Updated"
                    };

                case "Delete":
                    Console.WriteLine("[DATABASE]");
                    ConsoleColor originalColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("レコードを削除中...");

                    repository.Delete(request.Id);
                    Console.WriteLine("削除が完了しました。");
                    Console.ForegroundColor = originalColor;

                    return new ApiResponse
                    {
                        Success = true,
                        Message = "Deleted"
                    };

                default:

                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid Action"
                    };
            }
        }

    }
}