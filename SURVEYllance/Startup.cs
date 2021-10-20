using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SURVEYllance.Hubs;
using SURVEYllance.Resources;

namespace SURVEYllance
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddSignalR();
            services.AddSingleton<ISurveyRepository>(new SurveyRepository());
            services.AddSingleton(new Random());

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/webroot";
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
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            
            app.UseRouting();
            
            //Add endpoints
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
                //TODO: Add check if room exists
                endpoints.MapGet("/room/{JoinId}",
                    async context =>
                    {
                        var joinIdObj = context.Request.RouteValues["JoinId"];
                        if (joinIdObj == null)
                        {
                            context.Response.StatusCode = (int) HttpStatusCode.PaymentRequired;
                            await context.Response.WriteAsync("Invalid Join-ID");
                            throw new NullReferenceException("Join-ID is null");
                        }

                        string joinId = joinIdObj.ToString();
                        context.Response.StatusCode = (int) HttpStatusCode.OK;
                        await context.Response.WriteAsync($"Join-ID: {joinId}");
                    });
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<CreatorHub>("creator");
            });
            
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                
            });
        }
    }
}