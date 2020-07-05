using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vega.Controllers;
using Vega.Core;
using Vega.Core.Models;
using Vega.Persistence;

namespace Vega
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
            services.AddMvc();

            // 1. Add Authentication Services
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = "https://dev-7bdu8tf8.eu.auth0.com/";
                options.Audience = "https://api.vega.com";
            });

            ///auth0 policy service
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AppPolicies.RequireAdminRole, policy => policy.RequireClaim("https://vega.com/roles", "Admin"));
            });

            services.Configure<PhotoSettings>(Configuration.GetSection("PhotoSettings"));
            //Configuration.GetSection("PhotoSettings").GetChildren();

            //implementing dependency injection for interfaces
            services.AddScoped<IVehicleRepository, VehicleRepository>();

            //injection for photo interface
            services.AddScoped<IPhotoRepository, PhotoRepository>();
            //dependency injection interface
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //no memory storage
            services.AddTransient<IPhotoService, PhotoService>();
            services.AddTransient<IPhotoStorage, FileSystemPhotoStorage>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder=> builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        /*.AllowCredentials()*/);
            });
            /*services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
            });*/


            services.AddAutoMapper(typeof(Startup));

            services.AddDbContext<VegaDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Default")));
            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");
            //app.UseCors(options => options.AllowAnyOrigin());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            

            app.UseAuthorization();
            
            app.UseStaticFiles();

            app.UseAuthentication();

          

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
