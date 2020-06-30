using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using NLog;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SwaggerAAD.Helpers
{
    public class HandleExceptionAttribute : ExceptionFilterAttribute
    {
        ILogger logger;

        public HandleExceptionAttribute() : base()
        {
            logger = NLogBuilder.ConfigureNLog("nlog.xml").GetCurrentClassLogger();
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            var controller = context.RouteData.Values.Where(x => x.Key == "controller").FirstOrDefault();

            var req = context.HttpContext.Request;
            //req.EnableBuffering();

            object exception = null;
            if (req.Method.Equals("POST") || req.Method.Equals("PUT"))
            {
                string requestBody = string.Empty;
                using (var stream = new MemoryStream())
                {
                    req.Body.Seek(0, SeekOrigin.Begin);
                    await req.Body.CopyToAsync(stream);
                    requestBody = Encoding.UTF8.GetString(stream.ToArray());
                    req.Body.Seek(0, SeekOrigin.Begin);
                }

                logger.Error(context.Exception, $"{controller.Value}Controller|Method={req.Method}|Path={req.Path.Value}|Body={requestBody} ");
            }
            else if (req.Method.Equals("GET") || req.Method.Equals("DELETE"))
            {
                logger.Error(context.Exception, $"{controller.Value}Controller|Method={req.Method}|Path={req.Path.Value}");
            }

            exception = new
            {
                Message = context.Exception.Message,
                InnerMessage = context.Exception.InnerException?.Message ?? null
            };

            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(exception), Encoding.UTF8, "application/json"),
                //ReasonPhrase = "User is not authorized. Please contact your administrator."
            };
            //context.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "User is not authorized. Please contact your administrator.";
            context.Result = new HttpResponseMessageResult(response);
            base.OnException(context);
        }
    }
}
