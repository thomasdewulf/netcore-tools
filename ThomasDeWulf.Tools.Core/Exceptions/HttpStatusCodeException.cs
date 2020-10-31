using System;
using System.Net;

namespace ThomasDeWulf.Tools.Core.Exceptions
{
    public class HttpStatusCodeException: Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ContentType { get; set; } = "application/json";
        
        public HttpStatusCodeException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCodeException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}