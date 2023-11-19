using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            List<Product> featured = _context.Products.Include(p=>p.ProductImages).OrderBy(p => p.SellCount).ToList();
            List<Product> products = _context.Products.Include(p=>p.ProductImages).ToList();
            List<Product> bestseller = _context.Products.Include(p => p.ProductImages).OrderByDescending(p=>p.SellCount).ToList();
            List<Product> latest = _context.Products.Include(p => p.ProductImages).OrderByDescending(p=>p.Id).ToList();

            HomeVM vm = new() { Featured=featured,Bestseller=bestseller,Latest=latest, Products=products };

            return View(vm);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
