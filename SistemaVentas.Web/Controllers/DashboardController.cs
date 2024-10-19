using Microsoft.AspNetCore.Mvc;

namespace SistemaVentas.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
