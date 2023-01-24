using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VGTDataStore.Core;
using VGTDataStore.Core.Interfaces;
using VGTDataStore.InMemory;

namespace VGTServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(
                    CompatibilityVersion.Version_2_2);

            services.AddSingleton<IVGTUserDataStore>(
                new VGTUserDataStore());

            services.AddSingleton<IGameSessionsDataStore>(
                new VGTGameSessionsDataStore());

            services.AddSingleton<IPlayingCardsDataStore>(
                new VGTPlayingCardsDataStore());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();

            app.UseMvc();
        }
    }
}
