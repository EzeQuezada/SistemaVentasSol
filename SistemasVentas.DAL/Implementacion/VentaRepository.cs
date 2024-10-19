

using Microsoft.EntityFrameworkCore;
using SistemasVentas.DAL.Interfaces;
using SistemaVentas.DAL.Context;
using SistemaVentas.Entity;

namespace SistemasVentas.DAL.Implementacion
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        private readonly DBVENTAContext context;

        public VentaRepository(DBVENTAContext context) : base(context) 
        {
            this.context = context;
        }
        public async Task<Venta> Registrar(Venta entity)
        {
            Venta VentaGenerada = new Venta();
           using(var transaction = context.Database.BeginTransaction()) 
           {
                try
                {
                    foreach(DetalleVenta dv in entity.DetalleVenta)
                    {
                        Producto producto_encontrado = context.Productos.
                            Where(p=>p.IdProducto==dv.IdProducto).First();

                        producto_encontrado.Stock = producto_encontrado.Stock - dv.Cantidad;
                        context.Productos.Update(producto_encontrado);
                    }
                    await context.SaveChangesAsync();

                    NumeroCorrelativo numeroCorrelativo = context.NumeroCorrelativos.Where(n => n.Gestion == "Venta").First();
                    numeroCorrelativo.UltimoNumero = numeroCorrelativo.UltimoNumero + 1;
                    numeroCorrelativo.FechaActualizacion = DateTime.Now;

                    context.NumeroCorrelativos.Update(numeroCorrelativo);
                    await context.SaveChangesAsync();

                    string ceros = string.Concat(Enumerable.Repeat("0", numeroCorrelativo.CantidadDigitos.Value));
                    string numeroVenta = ceros + numeroCorrelativo.UltimoNumero.ToString();
                    numeroVenta.Substring(numeroVenta.Length - numeroCorrelativo.CantidadDigitos.Value,
                                                               numeroCorrelativo.CantidadDigitos.Value);

                    entity.NumeroVenta = numeroVenta;  

                    await context.Venta.AddAsync(entity);
                    await context.SaveChangesAsync();

                    VentaGenerada = entity;
                     transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
           }
            return VentaGenerada;
        }

        public async Task<List<DetalleVenta>> Reporte(DateTime FechaInicio, DateTime FechaFin)
        {
            List<DetalleVenta>listaResumen = await context.DetalleVenta
                .Include(v =>v.IdVentaNavigation)
                .ThenInclude(v =>v.IdUsuarioNavigation)
                .Include(v=>v.IdVentaNavigation)
                .ThenInclude(tdv=>tdv.IdTipoDocumentoVentaNavigation)
                .Where(dv=>dv.IdVentaNavigation.FechaRegistro.Value.Date>=FechaInicio.Date
                && dv.IdVentaNavigation.FechaRegistro.Value.Date<=FechaFin.Date).ToListAsync();
            return listaResumen;
                
        }
        
    }
}
