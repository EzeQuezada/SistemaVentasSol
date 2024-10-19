

using SistemaVentas.Entity;

namespace SistemasVentas.DAL.Interfaces
{
    public interface IVentaRepository  : IGenericRepository<Venta>
    {
       Task<Venta> Registrar(Venta entity);

        Task<List<DetalleVenta>> Reporte(DateTime FechaInicio, DateTime FechaFin);
    }
}
