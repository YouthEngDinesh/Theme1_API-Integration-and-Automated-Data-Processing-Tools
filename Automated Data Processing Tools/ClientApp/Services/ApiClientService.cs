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
        private readonly HttpClient _client = new();

        internal async Task<ApiResponse?>
            SendAsync(
                ClientRequest request)
            {
            Log.Information( "Sending Request: {Action}",request.Action);

            string json =
                    JsonSerializer.Serialize(
                        request);

                var content =
                    new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json");

                var response =
                    await _client.PostAsync(
                        "http://localhost:8080/",
                        content);
            Log.Information("Response Status: {Status}",response.StatusCode);

            string responseJson =
                await response.Content
                    .ReadAsStringAsync();

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
