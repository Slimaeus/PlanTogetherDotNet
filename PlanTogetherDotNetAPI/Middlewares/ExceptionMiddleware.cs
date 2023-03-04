using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace PlanTogetherDotNetAPI.Middlewares
{
    public class ExceptionMiddleware : OwinMiddleware
    {
        public ExceptionMiddleware(OwinMiddleware next) : base(next)
        {
        }
        public override async Task Invoke(IOwinContext context)
        {
            string correlationId = Guid.NewGuid().ToString();
            try
            {
                await Next.Invoke(context);
            }
            catch (Exception exception)
            {
                var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                //Log.ForContext("CorrelationId", correlationId)
                //    .Fatal(exception, "An unhandled exception occurred during the request processing. Correlation ID: {CorrelationId}", correlationId);

                var statusCode = (int)HttpStatusCode.InternalServerError;

                dynamic error;
                if (isDevelopment == "Development")
                {
                    var stackTrace = exception.StackTrace
                    .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    error = new
                    {
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                        Title = "Internal Server Error",
                        Status = statusCode,
                        Instance = context.Request.Path.Value,
                        CorrelationId = correlationId,
                        Details = exception.Message,
                        StackTrace = stackTrace
                    };
                }
                else
                {
                    error = new
                    {
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                        Title = "Internal Server Error",
                        Status = statusCode,
                        Instance = context.Request.Path.Value,
                        CorrelationId = correlationId,
                    };
                }

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = statusCode;

                var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                var json = JsonConvert.SerializeObject(error, Formatting.None, settings);

                using (var writer = new StreamWriter(context.Response.Body))
                {
                    writer.Write(json);
                }
            }
        }
    }
}