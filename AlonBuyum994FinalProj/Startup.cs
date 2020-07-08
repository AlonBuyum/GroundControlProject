using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlonBuyum994FinalProj.Hubs;
using DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repositories;

namespace AlonBuyum994FinalProj
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
            services.AddSingleton<ISimulatorService, SimulatorService>();
            services.AddSingleton<ILogicService, LogicService>();
            services.AddSingleton<ITimerService, TimerService>();
            services.AddDbContext<AirportContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("Airport_FinalProjDB")),
                ServiceLifetime.Singleton);
            services.AddSingleton<IDataService, DataService>();
            services.AddSignalR();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AirportContext airportContext)
        {
            airportContext.Database.EnsureCreated();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<AirportHub>("/airportHub");
            });
        }
    }
}
