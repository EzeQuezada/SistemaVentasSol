using Microsoft.AspNetCore.Mvc;

namespace SistemaVentas.Web.Controllers
{
    public class ReporteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
