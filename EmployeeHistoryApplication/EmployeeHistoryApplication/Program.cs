using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EmployeeHistoryApplication.Data;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EmployeeHistoryApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeHistoryApplicationContext")
    ?? throw new InvalidOperationException("Connection string 'EmployeeHistoryApplicationContext' not found.")));

builder.Services.AddControllersWithViews();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-GB"),
        new CultureInfo("mk-MK"),
         new CultureInfo("sq-AL"),
    };

    options.DefaultRequestCulture = new RequestCulture("mk-MK", "mk-MK");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
