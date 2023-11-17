using Microsoft.EntityFrameworkCore;
using ProniaTask.DAL;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt=>
opt.UseSqlServer("server=WINDOWS-DPJDVHL\\SQLEXPRESS;database=ProniaTask;trusted_connection=true;integrated security=true"));
var app = builder.Build();


app.UseStaticFiles();

app.UseRouting();
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
