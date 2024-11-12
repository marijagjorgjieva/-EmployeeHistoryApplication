using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EmployeeHistoryApplication.Data;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EmployeeHistoryApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeHistoryApplicationContext") ?? throw new InvalidOperationException("Connection string 'EmployeeHistoryApplicationContext' not found.")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

//za errors?
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//od http vo https
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//ja run apliakcijata
app.Run();
