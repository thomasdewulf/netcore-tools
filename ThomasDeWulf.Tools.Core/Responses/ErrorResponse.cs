using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ThomasDeWulf.Tools.Core.Responses
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        
        public string DetailedMessage { get; set; }
        
        public string StackTrace { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}