using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OBRS.Areas.Identity.Data;
using OBRS.Data;
using OBRS.Models;
namespace OBRS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("OBRSContextConnection") ?? throw new InvalidOperationException("Connection string 'OBRSContextConnection' not found.");

            builder.Services.AddDbContext<OBRSContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<OBRSUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>().AddEntityFrameworkStores<OBRSContext>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var provider = builder.Services.BuildServiceProvider();
            var config = provider.GetRequiredService<IConfiguration>();
            builder.Services.AddDbContext<obrsContext>(item => item.UseSqlServer(
                config.GetConnectionString("OBRSContextConnection")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}
