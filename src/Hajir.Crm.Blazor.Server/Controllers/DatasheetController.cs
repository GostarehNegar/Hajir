using Microsoft.AspNetCore.Mvc;

namespace Hajir.Crm.Blazor.Server.Controllers
{
    public class DatasheetController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
