
using SistemaVentas.Web.Utilidades.Automapper;

using SistemaVentas.IOC.Dependencias;
using Microsoft.EntityFrameworkCore;
using SistemaVentas.DAL.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DBVENTAContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBVENTAContext")));

builder.Services.InyectarDependencia(builder.Configuration);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

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

app.Run();
