
using SistemaVentas.Entity;

namespace SistemasVentas.BLL.Interfaces
{
    public interface IRolService
    {
        Task<List<Rol>> Lista();

    }
}
