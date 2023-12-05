using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using DeliverySystem.Module;
using DeliverySystem.Interface;
using DeliverySystem.Variables.Repository;

namespace DeliverySystem
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
            var connectionData = Configuration.GetSection("ExampleDB").Get<SqlConnectionInfo>();
            IRepositoryOperater repository = new RepositoryService(connectionData.GetSqlConnectionStringBuilder());
            services.AddSingleton<IRepositoryOperater>(repository);

            IThirdPartyAPIOperater thirdPartyAPIOperater = new ThirdPartyAPIService();
            services.AddSingleton<IThirdPartyAPIOperater>(thirdPartyAPIOperater);

            ITaskDataService taskService = new TaskDataService(repository);
            services.AddSingleton<ITaskDataService>(taskService);

            //ILog log = new Logger();
            //services.AddSingleton<ILog>(log);

            services.AddSignalR();
            services.AddControllers();
            services.AddHostedService<TaskServiceBackgroundWork>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Delivery System API",
                    Description = "A simple example ASP.NET Core Web API",
                    Contact = new OpenApiContact
                    {
                        Name = "Joanne Lee",
                        Email = "lca8012@gmail.com",
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Delivery System V1 API");
            });
            
            app.UseCors(builder =>
            {
                builder.AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials();
            });

            app.UseRouting();            

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SignalRHub>("/chatHub");
            });

           
        }
    }
}
