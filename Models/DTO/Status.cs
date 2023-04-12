using System;
namespace AuthApi.Models.DTO
{
    public class Status
    {
        public Status()
        {
            
        }
        public Status(int status1, string status_message1, Object data1)
        {
            StatusCode = status1;
            Message = status_message1;
            data = data1;
        }

        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public Object data { get; set; }
    }
}
