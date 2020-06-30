using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SwaggerAAD.Helpers;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace SwaggerAAD
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SwaggerAAD API", Version = "v1", Description = "ASP.NET Web Api Core 3.1" });
                var xmlPath = AppDomain.CurrentDomain.BaseDirectory + @"SwaggerAAD.xml";
                c.IncludeXmlComments(xmlPath);

                OpenApiSecurityScheme openApiSecurityScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{Configuration.GetValue<string>("AzureAd:TenantId")}/oauth2/authorize"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "read", "Access API" }
                            }
                        }
                    },
                };

                c.AddSecurityDefinition("oauth2", openApiSecurityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement() { {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                    },
                    new[] { "read" } } });
            });

            services.AddMvcCore(options => options.Filters.Add(new HandleExceptionAttribute()));
            services.ConfigureServices();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // configure strongly typed azureAd object
            var azureAd = Configuration.GetSection("AzureAd");
            services.Configure<AzureAd>(azureAd);

            services.AddAuthentication(AzureADDefaults.JwtBearerAuthenticationScheme)
                .AddAzureADBearer(options => azureAd.Bind(options));

            services.Configure<JwtBearerOptions>(AzureADDefaults.JwtBearerAuthenticationScheme, options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = true,
                    ValidAudiences = new string[] { options.Audience, $"api://{options.Audience}" },
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                var clientId = Configuration.GetValue<string>("AzureAd:ClientId");
                c.OAuthClientId(clientId);
                c.OAuthClientSecret(Configuration.GetValue<string>("Swagger:ClientSecret"));
                c.OAuthRealm(clientId);
                c.OAuthAppName("SwaggerAAD API v1");
                c.OAuthScopeSeparator(" ");
                c.OAuthAdditionalQueryStringParams(new Dictionary<string, string>() { { "resource", clientId } });
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SwaggerAAD API v1");
                c.DocExpansion(DocExpansion.None);
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
