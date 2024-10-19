using Microsoft.AspNetCore.Mvc;

namespace SistemaVentas.Web.Controllers
{
    public class NegocioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
