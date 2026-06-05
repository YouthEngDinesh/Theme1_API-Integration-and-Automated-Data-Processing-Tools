using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp.Models
{
    class ClientRequest
    {
        //Action Id DeviceLog 

        public string Action { get; set; } = string.Empty;

        public int Id { get; set; }

        public DeviceLog? DeviceLog { get; set; }

    }
}
