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
        
        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Neo4J connection

            var neo4JClient = new BoltGraphClient(new Uri("bolt://localhost:7687"), "neo4j", "12345678");
            neo4JClient.ConnectAsync();
            services.AddSingleton<IGraphClient>(neo4JClient);

            

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FoodExplorer", Version = "v1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); 
            });
            services.AddMemoryCache();

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            services.AddSignalR();
            services.AddLogging();

            services.AddCors( options=>{
                options.AddPolicy("CORS",builder=>
                {
                    builder.WithOrigins("http:localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    builder.WithOrigins(new string[]
                    {
                        "http://localhost:8080",
                        "http://localhost:8080",
                        "http://127.0.0.1:8080",
                        "http://127.0.0.1:8080",
                        "http://localhost:5121",
                        "http://127.0.0.1:5501",
                        "http://localhost:5001",
                        "http://127.0.0.1:5001",
                        "http://127.0.0.1:3000",
                        "http://localhost:3000",
                        "http://127.0.0.1:3000",
                        "http://localhost:3000",
                        "http:localhost:3000"
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FoodExplorer v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CORS");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
