using SchoolDemo.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchoolDemo.Models.Interfaces;
using SchoolDemo.Services;

namespace SchoolDemo
{
  public class Startup
  {
    // 1. Add a property to hold our configuration
    public IConfiguration Configuration { get; }

    // 2. Add a constructor to receive our condfiguration (through some magic)
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    // This is very similar to what dotenv did for us in NodeJS
    public void ConfigureServices(IServiceCollection services)
    {
      // 3. Register our DbContext with the app
      services.AddDbContext<SchoolDbContext>(options =>
      {
        // Equivalent to DATABASE_URI
        string connectionString = Configuration.GetConnectionString("DefaultConnection");
        options.UseSqlServer(connectionString);
      });

      // Register my Dependency Injection services
      // This mapps the Dependency (IStudent) to the correct Service (StudentRepository)
      // "Whenever I see IStudent, use StudentRepository
      // This makes StudentRepository swappapble with any alternate implementation
      services.AddTransient<ICourse, CourseRepository>();
      services.AddTransient<IStudent, StudentRepository>();
      services.AddTransient<ITechnology, TechnologyRepository>();

      // Bring in our controllers
      services.AddMvc();
      services.AddControllers();


      services.AddControllers().AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
      );
      services.AddSwaggerGen(options =>
      {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
        {
          Title = "School Demonstration Repo",
          Version = "v1"
        });
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseSwagger(options =>
      options.RouteTemplate = "/api/{documentName}/swagger.json");

      app.UseSwaggerUI(options =>
     {
       options.SwaggerEndpoint("/api/v1/swagger.json", "UI SchoolDemo");
       options.RoutePrefix = "";
     });

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();        
      });
    }
  }
}
