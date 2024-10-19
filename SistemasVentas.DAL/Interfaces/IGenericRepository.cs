using System.Linq.Expressions;

namespace SistemasVentas.DAL.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filter);
        Task<TEntity> Crear(TEntity entity);
        Task<bool> Editar(TEntity entity);
        Task<bool> Eliminar(TEntity entity);
        Task<IQueryable<TEntity>>Consultar(Expression<Func<TEntity, bool>> filter=null);
    }
}
