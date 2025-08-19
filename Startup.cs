using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using Neo4jClient;

namespace FoodExplorer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Neo4J connection
            var neo4JClient = new BoltGraphClient(new Uri("bolt://localhost:7687"), "neo4j", "12345678");
            neo4JClient.ConnectAsync();
            services.AddSingleton<IGraphClient>(neo4JClient);

            services.AddControllers();

            // Swagger konfiguracija
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FoodExplorer API", Version = "v1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            services.AddMemoryCache();

            services.AddSignalR();
            services.AddLogging();

            // CORS konfiguracija
            services.AddCors(options =>
            {
                options.AddPolicy("CORS", builder =>
                {
                    builder.WithOrigins(
                        "http://localhost:8080",
                        "http://127.0.0.1:8080",
                        "http://localhost:5121",
                        "http://127.0.0.1:5501",
                        "http://localhost:5001",
                        "http://127.0.0.1:5001",
                        "http://localhost:3000",
                        "http://127.0.0.1:3000"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Ako si u Developmentu, možeš zadržati developer exception page
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CORS");

            app.UseAuthorization();

            // Swagger uvek uključen
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FoodExplorer API v1");
                c.RoutePrefix = "swagger"; // Swagger dostupan na /swagger
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
