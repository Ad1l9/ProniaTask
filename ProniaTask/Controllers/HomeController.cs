using Microsoft.AspNetCore.Mvc;
using ProniaTask.DAL;
using ProniaTask.HomeViewModel;
using ProniaTask.Models;

namespace ProniaTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            this._context = context;
        }

        public IActionResult Index()
        {

            List<Product> featured = _context.Products.OrderBy(p=>p.Name).ToList();
            List<Product> bestseller = _context.Products.OrderByDescending(p=>p.SellCount).ToList();
            List<Product> latest = _context.Products.OrderByDescending(p=>p.Id).ToList();

            HomeVM vm = new() { Featured=featured,Bestseller=bestseller,Latest=latest };

            return View(vm);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
