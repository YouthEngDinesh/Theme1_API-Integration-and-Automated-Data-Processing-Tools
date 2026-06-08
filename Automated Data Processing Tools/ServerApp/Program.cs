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
            var repository = new DeviceLogRepository();
            HttpListener listener = new HttpListener();

            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();
            Console.WriteLine("=================================================");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" ServerApp Started");
            Console.WriteLine("=================================================");
            Console.WriteLine($" Port      : 8080");
            Console.WriteLine($" StartedAt : {DateTime.Now}");
            Console.WriteLine(" Status    : Waiting for client requests...");
            Console.ResetColor();
            Console.WriteLine("=================================================");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("Waiting for next request...");
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
                    Console.WriteLine("Adding new record...");
                    repository.Add(request.DeviceLog);

                    Console.WriteLine("Insert completed.");
                    Console.ResetColor();
                    return new ApiResponse
                    {
                        Success = true,
                        Message = "Added"
                    };

                case "GetAll":

                    Console.WriteLine("[DATABASE]");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Fetching all records...");

                    var records = repository.GetAll();
                    Console.WriteLine($"Records Found : {records.Count}");
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
                    Console.WriteLine("Updating record...");

                    repository.Update(request.DeviceLog);
                    
                    Console.WriteLine("Update completed.");
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
                    Console.WriteLine("Deleting record...");

                    repository.Delete(request.Id);
                    Console.WriteLine("Delete completed.");
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