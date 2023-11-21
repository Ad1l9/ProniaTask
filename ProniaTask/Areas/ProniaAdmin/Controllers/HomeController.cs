using Microsoft.AspNetCore.Mvc;
using ProniaTask.DAL;
using ProniaTask.ViewModel;

namespace ProniaTask.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
