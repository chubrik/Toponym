using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Toponym.Site.Services;

namespace Toponym.Site {
    public class Startup {

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // Add framework services.
            services.AddMvc();
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
            services.AddSingleton<DataService>();
            services.AddSingleton<ItemService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DataService dataService) {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            //else
            //    app.UseExceptionHandler("/Home/Error");

            app.UseStaticFiles();

            app.UseMvc();
            //routes => {
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }
}
