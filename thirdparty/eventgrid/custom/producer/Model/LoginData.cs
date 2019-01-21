using System;

namespace linuxacademy.az203.thidparty.eventgrid
{
    public class LoginData
    {
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public String Username { get; set; }
    }
}