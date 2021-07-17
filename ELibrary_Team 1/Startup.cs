using ELibrary.Data;
using ELibrary_Team_1.Models;
using ELibrary_Team1.DataAccess.Data.Repository;
using ELibrary_Team1.DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ELibrary_Team_1
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
            services.AddControllersWithViews();
            ///
            services.AddDbContext<ELibraryDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ElibraryDb")));




            services.AddDefaultIdentity<AppUser>()
                .AddEntityFrameworkStores<ELibraryDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();


            // Repository and Service
            services.AddRepositoryServices();


            //
            services.AddScoped<IUnitOfWork, UnitOfWork>();



            services.AddRazorPages();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Unauthenticated}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "api",
                    pattern: "{area=Authenticated}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
