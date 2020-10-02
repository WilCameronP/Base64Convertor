using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Base64Convertor
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
            var fectp = new FileExtensionContentTypeProvider();
            fectp.Mappings.TryAdd(".zpl", "text/plain");
            services.AddSingleton<IContentTypeProvider>(fectp);

            //services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddJsonLocalization(options => options.ResourcesPath = "Resources");
            services.AddControllersWithViews()
                // Add support for finding localized views, based on file name suffix, e.g. Index.fr.cshtml
                //.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)

                // Add support for localizing strings in data annotations (e.g. validation messages) via the IStringLocalizer abstractions.
                .AddDataAnnotationsLocalization()
                //.AddViewLocalization()
                .AddMvcLocalization();

            services.AddMemoryCache();
            services.AddJsonLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("zh-CN"),
                    new CultureInfo("ja-JP")
                };
                options.DefaultRequestCulture = new RequestCulture(culture: "zh-CN", uiCulture: "zh-CN");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Add(new CustomRequestCultureProvider(ctx =>
                {
                    
                    var currentCulture = "en";
                    var segments = ctx.Request.Path.Value.Split(new char[] { '/' },
                        StringSplitOptions.RemoveEmptyEntries);

                    if (segments.Length > 1 && segments[0].Length == 2)
                    {
                        currentCulture = segments[0];
                    }

                    var requestCulture = new ProviderCultureResult(currentCulture);

                    return Task.FromResult(requestCulture);
                }));


                // You can change which providers are configured to determine the culture for requests, or even add a custom

                // provider with your own logic. The providers will be asked in order to provide a culture for each request,

                // and the first to provide a non-null result that is in the configured supported cultures list will be used.

                // By default, the following built-in providers are configured:

                // - QueryStringRequestCultureProvider, sets culture via "culture" and "ui-culture" query string values, useful for testing

                // - CookieRequestCultureProvider, sets culture via "ASPNET_CULTURE" cookie

                // - AcceptLanguageHeaderRequestCultureProvider, sets culture via the "Accept-Language" request header

                //options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context =>

                //{

                //  // My custom request culture logic

                //  return new ProviderCultureResult("en");

                //}));

            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{culture=zh-CN}/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
