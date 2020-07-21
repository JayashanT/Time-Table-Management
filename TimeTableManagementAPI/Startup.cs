﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TimeTableAPI.Models;
using TimeTableManagementAPI.Hub;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.Repository;
using TimeTableManagementAPI.Services;

namespace TimeTableManagementAPI
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSignalR();

            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<ITimeTableServices, TimeTableServices>();
            services.AddScoped<ICommonRepository<Users>, CommonRepository<Users>>();
            services.AddScoped<ICommonRepository<LoginVM>, CommonRepository<LoginVM>>();
            services.AddScoped<ICommonRepository<Class>, CommonRepository<Class>>();
            services.AddScoped<ICommonRepository<Time_Table>, CommonRepository<Time_Table>>();
            services.AddScoped<ICommonRepository<Subject>, CommonRepository<Subject>>();
            services.AddScoped<ICommonRepository<Teacher_Subject>, CommonRepository<Teacher_Subject>>();
            services.AddScoped<ICommonRepository<Slot>, CommonRepository<Slot>>();
            services.AddScoped<ICommonRepository<Attendance>, CommonRepository<Attendance>>();
            services.AddScoped<ICommonRepository<Resource>, CommonRepository<Resource>>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Configuration["Jwt:Issuer"],
            ValidAudience = Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
        };
    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(options =>
            options.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSignalR(routes =>
            {
                routes.MapHub<ConnectionHub>("/connectionHub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
