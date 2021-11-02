using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly SurveyRepository _surveyRepository;
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _surveyRepository = new SurveyRepository();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddSignalR();
            services.AddSingleton<ISurveyRepository>(_surveyRepository);
            

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
                endpoints.MapGet("/room/{JoinId?}",
                    async context =>
                    {
                        var joinId = context.Request.RouteValues["JoinId"] as string;
                        if (joinId == null)
                        {
                            context.Response.StatusCode = (int) HttpStatusCode.UnprocessableEntity;
                            await context.Response.WriteAsync("Invalid Join-ID");
                            throw new NullReferenceException("Join-ID is null");
                        }
                        
                        if (joinId == "Teapot:)")
                        {
                            context.Response.StatusCode = 418;
                            await context.Response.WriteAsync("You found me. I#m a teapot");
                            return;
                        }

                        if (_surveyRepository.RunningSessions.FirstOrDefault(room => room.JoinId == joinId) != null)
                        {
                            context.Response.StatusCode = (int) HttpStatusCode.OK;
                            await context.Response.WriteAsync($"Room {joinId} exists");
                        }
                        else
                        {
                            context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                            await context.Response.WriteAsync($"Room {joinId} does not exist");
                        }
                        //TODO: Else HttpStatusCode.Gone for deleted rooms
                        
                        
                    });
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<CreatorHub>("creator");
                endpoints.MapHub<ParticipantHub>("participant");
            });
            
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                
            });
        }
    }
}