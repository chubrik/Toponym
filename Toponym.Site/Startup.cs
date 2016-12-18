using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Toponym.Site.Services;

namespace Toponym.Site {
    public class Startup {

        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc();
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
            services.AddSingleton<DataService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
