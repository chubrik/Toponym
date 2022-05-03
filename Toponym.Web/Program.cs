using Microsoft.AspNetCore.StaticFiles;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Toponym.Web;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddSingleton<DataService>();

// https://github.com/aspnet/HttpAbstractions/issues/315
services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

services.AddRazorPages()
#if DEBUG
    .AddRazorRuntimeCompilation()
#endif
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
        //options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

//

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // https://github.com/aspnet/Announcements/issues/432
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

#if !DEBUG
app.UseHttpsRedirection();
#endif

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var contentTypeProvider = new FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".css"] = "text/css; charset=utf-8";
contentTypeProvider.Mappings[".js"] = "application/javascript; charset=utf-8";
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = contentTypeProvider });

app.UseRouting();
app.MapRazorPages();

app.Run();
