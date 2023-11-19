using Microsoft.EntityFrameworkCore;
using ProniaTask.DAL;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt=>
opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
var app = builder.Build();


app.UseStaticFiles();

app.UseRouting();
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
