using System;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using SugarCenter.Classes;
using SugarCenter.Helpers;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Data;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Interfaces;

namespace SugarCenter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public const string CookieScheme = "YourSchemeName";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<SugaringCentreAucklandElkContext>(x =>
                x.UseSqlServer(Configuration.GetConnectionString("DsElkConnection")));

            services.AddTransient<ISugaringCentreAucklandElkRepository, SugaringCentreAucklandElkRepository>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //services.AddDistributedMemoryCache();

            services.AddMemoryCache();
            
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Sets the default scheme to cookies
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/AdminConsole/Denied";
                    options.LoginPath = "/AdminConsole/Login";
                });

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });
            
            services.AddSingleton<IConfigureOptions<CookieAuthenticationOptions>, ConfigureMyCookie>();
            
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(120);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            var instagramBasicDisplayApiSettings = new InstagramBasicDisplayAPISettings();
            Configuration.Bind("InstagramBasicDisplayAPISettings", instagramBasicDisplayApiSettings);
            ConfigureInstagramBasicDispalyAPIHelper(instagramBasicDisplayApiSettings);
        }

        private void ConfigureInstagramBasicDispalyAPIHelper(InstagramBasicDisplayAPISettings settings)
        {
            InstagramBasicDisplayAPIHelper.AppId = settings.AppId.ToString();
            InstagramBasicDisplayAPIHelper.AppSecret = settings.AppSecret;
            InstagramBasicDisplayAPIHelper.InstagramRedirectUrl = settings.InstagramRedirectUrl;
            InstagramBasicDisplayAPIHelper.ApiBaseUrl = settings.ApiBaseUrl;
            InstagramBasicDisplayAPIHelper.Scope = settings.Scope;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();

            app.UseAuthentication();
            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "deleteCategory",
                    template: "{controller=AdminConsole}/{action=DeleteCategory}/{id?}");
            });
        }
    }
}
