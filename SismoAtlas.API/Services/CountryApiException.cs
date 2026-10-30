using System.Net;

namespace SismoAtlas.API.Services
{
    public class CountryApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public CountryApiException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}