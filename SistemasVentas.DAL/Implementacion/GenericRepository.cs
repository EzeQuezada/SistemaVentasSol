using SistemaVentas.DAL.Context;
using SistemasVentas.DAL.Interfaces;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SistemasVentas.DAL.Implementacion
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DBVENTAContext context;

        public GenericRepository(DBVENTAContext context)
        {
            this.context = context;
        }
        public async Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                TEntity entity = await context.Set<TEntity>().FirstOrDefaultAsync(filter);
                return entity;
            }
            catch 
            {
             throw;
            }
        }
        public async Task<TEntity> Crear(TEntity entity)
        {
            try
            {
                context.Set<TEntity>().Add(entity);
                await context.SaveChangesAsync();
                return entity;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Editar(TEntity entity)
        {
            try
            {
                context.Update(entity);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(TEntity entity)
        {
            try
            {
                context.Remove(entity);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> queryEntidad = filter == null ? context.Set<TEntity>():context.Set<TEntity>().Where(filter);
            return queryEntidad;
        }

     

      

       

       
    }
}
