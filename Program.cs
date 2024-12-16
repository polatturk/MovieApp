using Business;
using Helpers;
using System.Data.SqlClient;

namespace Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(15);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
		IConfiguration configuration = builder.Configuration;
		builder.Services.AddSingleton<SqlConnection>(provider =>
		{
			var configuration = provider.GetRequiredService<IConfiguration>();
			var connectionString = configuration.GetConnectionString("DefaultConnection");
			return new SqlConnection(connectionString);
		});
		builder.Services.AddSingleton<IHelper, Helper>();
		builder.Services.AddSingleton<IAdminService, AdminService>();

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
        app.UseSession();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
