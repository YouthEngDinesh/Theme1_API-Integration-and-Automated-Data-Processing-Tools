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
            Console.WriteLine("Main Started");
            //Console.ReadLine();


            LoggerConfig.Configure();  // Serilogの設定を行う_Configure the Serilog
            var repository = new DeviceLogRepository();
            HttpListener listener = new HttpListener();

            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();
            Console.WriteLine("Server Started");

            while (true)
            {
                HttpListenerContext context =
               listener.GetContext();

                ProcessRequest(context, repository);

            }
        }

        static void ProcessRequest(
        HttpListenerContext context,
        DeviceLogRepository repository)
        {
            try
            {
                HttpListenerRequest request =
                    context.Request;

                HttpListenerResponse response =
                    context.Response;

                using var reader =
                    new StreamReader(
                        request.InputStream);

                string json =
                    reader.ReadToEnd();

                Log.Information(
                    "Request Received: {Json}",
                    json);

                var requestObject =
                    JsonSerializer.Deserialize
                    <ClientRequest>(json);

                ApiResponse apiResponse = HandleAction(requestObject, repository);

                string responseJson =
                    JsonSerializer.Serialize(
                        apiResponse);

                byte[] buffer =
                    Encoding.UTF8.GetBytes(
                        responseJson);

                response.ContentType =
                    "application/json";

                response.StatusCode = 200;

                response.ContentLength64 =
                    buffer.Length;

                response.OutputStream.Write(
                    buffer,
                    0,
                    buffer.Length);

                response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    "Server Error");
            }
        }

        static ApiResponse HandleAction(
        ClientRequest request,
        DeviceLogRepository repository)
        {
            switch (request.Action)
            {
                case "Add":

                    repository.Add(
                        request.DeviceLog);

                    return new ApiResponse
                    {
                        Success = true,
                        Message = "Added"
                    };

                case "GetAll":

                    return new ApiResponse
                    {
                        Success = true,
                        Message = "Records Returned",
                        Data = repository.GetAll()
                    };

                case "Update":

                    repository.Update(
                        request.DeviceLog);

                    return new ApiResponse
                    {
                        Success = true,
                        Message = "Updated"
                    };

                case "Delete":

                    repository.Delete(
                        request.Id);

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