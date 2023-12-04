using Microsoft.EntityFrameworkCore;
using ProniaTask.DAL;
using ProniaTask.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt=>
opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<LayoutService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();


app.UseStaticFiles();

app.UseRouting();
app.MapControllerRoute(
    "default",
    "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
