using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApplication.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using BlogApplication.Repository;
using Microsoft.AspNetCore.Identity;
using BlogApplication.Data.FileManager;
using Microsoft.AspNetCore.Mvc;

namespace BlogApplication
{
    public class Startup
    {
        private IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => { options.EnableEndpointRouting = false;
                                         options.CacheProfiles.Add("Monthly",
                                             new CacheProfile {
                                                 Duration = 60 * 60 * 24 * 7 * 4 });
                                                 });
            services.AddDbContext<AppDbContext>(options=>options.UseSqlServer(_configuration["DevConnection"]));
            services.AddTransient<IRepository,Repository.Repository>();
            services.AddTransient<IFileManager, FileManager>();
            services.AddIdentity<IdentityUser,IdentityRole>(options=> {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                })
                //.AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
            services.ConfigureApplicationCookie(options => { options.LoginPath = "/Auth/Login"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
            }
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseMvcWithDefaultRoute();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
        }
    }
}
