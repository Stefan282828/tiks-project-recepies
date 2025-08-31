using FoodExplorer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FoodExplorer.Services;


namespace FoodExplorer
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // PostgreSQL konekcija
            services.AddDbContext<FoodExplorerContext>(options =>
                options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();

            services.AddScoped<IKategorijaService, KategorijaService>();
            services.AddScoped<IPodkategorijaService, PodkategorijaService>();


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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("CORS");
            app.UseAuthorization();

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FoodExplorer API v1");
                c.RoutePrefix = "swagger";
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
