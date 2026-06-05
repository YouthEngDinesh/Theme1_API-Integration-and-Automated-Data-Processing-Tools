using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp.Models
{
    internal class ApiResponse
    {
        // bool Success   string Message    object Data

        public bool Success { get; set; }


        public string Message { get; set; } = string.Empty;


        public object? Data { get; set; }

    }
}
