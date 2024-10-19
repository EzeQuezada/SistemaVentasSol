using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SistemaVentas.DAL.Context;
using SistemasVentas.DAL.Implementacion;
using SistemasVentas.DAL.Interfaces;
using SistemasVentas.BLL.Implementaciones;
using SistemasVentas.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using SistemaVentas.Entity;



namespace SistemaVentas.IOC.Dependencias
{
    public static class Dependencias
    {
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DBVENTAContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("VentasContext"));
            });

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IVentaRepository, VentaRepository>();

            services.AddScoped<ICorreoService, CorreoService>();
            services.AddScoped<IFireBaseService, FireBaseService>();

            services.AddScoped<IUtilidadesService, UtilidadesService>();
            services.AddScoped<IRolService, RolService>();

            services.AddScoped<IUsuarioService, UsuarioService>();

        }
    }
}
