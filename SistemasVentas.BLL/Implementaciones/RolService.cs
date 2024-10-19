using SistemasVentas.BLL.Interfaces;
using SistemasVentas.DAL.Interfaces;
using SistemaVentas.Entity;

namespace SistemasVentas.BLL.Implementaciones
{
    public class RolService : IRolService
    {
        private readonly IGenericRepository<Rol> repositorio;

        public RolService(IGenericRepository<Rol> _repositorio)
        {
            repositorio = _repositorio;
        }
        public async Task<List<Rol>> Lista()
        {
            IQueryable<Rol> query = await repositorio.Consultar();
            return query.ToList();
        }
    }
}
