using Microsoft.AspNetCore.Mvc;

namespace SistemaVentas.Web.Controllers
{
    public class ProductoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
