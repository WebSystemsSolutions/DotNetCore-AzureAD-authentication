using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SwaggerAAD.Helpers
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
