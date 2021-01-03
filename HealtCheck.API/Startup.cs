using HealthCheck.API.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HealtCheck.API
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
            services.AddHealthChecks().AddCheck<ExampleHealthCheck>("parametre almayan healthCheck").AddTypeActivatedCheck<HealthChecksWithParameters>("parametre alan healthCheck", args: new object[] { "fatih", 4 }).AddSqlServer(Configuration.GetConnectionString("SqlCon"));

            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(1);
                options.Period = TimeSpan.FromSeconds(1);
                //  options.Predicate = (check) => check.Tags.Contains("ready");
            });

            services.AddSingleton<IHealthCheckPublisher, ConsolePublisher>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HealtCheck.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealtCheck.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
                {
                    ResponseWriter = (context, result) =>
                     {
                         context.Response.ContentType = "application/json; charset=utf-8";

                         var options = new JsonWriterOptions
                         {
                             Indented = true
                         };

                         using (var stream = new MemoryStream())
                         {
                             using (var writer = new Utf8JsonWriter(stream, options))
                             {
                                 writer.WriteStartObject();
                                 writer.WriteString("status", result.Status.ToString());
                                 writer.WriteStartObject("results");
                                 foreach (var entry in result.Entries)
                                 {
                                     writer.WriteStartObject(entry.Key);
                                     writer.WriteString("status", entry.Value.Status.ToString());
                                     writer.WriteString("description", entry.Value.Description);
                                     writer.WriteStartObject("data");
                                     foreach (var item in entry.Value.Data)
                                     {
                                         writer.WritePropertyName(item.Key);
                                         JsonSerializer.Serialize(
                                             writer, item.Value, item.Value?.GetType() ??
                                             typeof(object));
                                     }
                                     writer.WriteEndObject();
                                     writer.WriteEndObject();
                                 }
                                 writer.WriteEndObject();
                                 writer.WriteEndObject();
                             }

                             var json = Encoding.UTF8.GetString(stream.ToArray());

                             return context.Response.WriteAsync(json);
                         }
                     }
                }

                        );
                endpoints.MapControllers();
            });
        }
    }
}