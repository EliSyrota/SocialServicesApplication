using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SocialServicesApplication
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public Services Services { get; private set; }
		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();
			services.AddControllers();
			var telegramSettings = Configuration.GetSection("TelegramSettings");
			var timerSettings = Configuration.GetSection("Settings");
			services.Configure<TelegramSettings>(telegramSettings);
			services.Configure<Settings>(timerSettings);
			Services = new Services(timerSettings.Get<Settings>(), telegramSettings.Get<TelegramSettings>());
			services.AddSingleton(Services);
			// In production, the Angular files will be served from this directory
			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp/dist";
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
			}

			app.UseStaticFiles();
			if (!env.IsDevelopment())
			{
				app.UseSpaStaticFiles();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapControllerRoute(
						 name: "default",
						 pattern: "{controller}/{action=Index}/{id?}");
			});

			app.UseSpa(spa =>
			{
					// To learn more about options for serving an Angular SPA from ASP.NET Core,
					// see https://go.microsoft.com/fwlink/?linkid=864501

					spa.Options.SourcePath = "ClientApp";

				if (env.IsDevelopment())
				{
					spa.UseAngularCliServer(npmScript: "start");
				}
			});
		}
	}
}
