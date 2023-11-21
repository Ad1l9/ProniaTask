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

        public async Task<IActionResult> Index()
        {

            List<Product> featured =await  _context.Products.Include(p=>p.ProductImages).OrderBy(p => p.SellCount).ToListAsync();
            List<Product> products =await _context.Products.Include(p=>p.ProductImages).ToListAsync();
            List<Product> bestseller =await _context.Products.Include(p => p.ProductImages).OrderByDescending(p=>p.SellCount).ToListAsync();
            List<Product> latest =await _context.Products.Include(p => p.ProductImages).OrderByDescending(p=>p.Id).ToListAsync();

            HomeVM vm = new() { Featured=featured,Bestseller=bestseller,Latest=latest, Products=products };

            return View(vm);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
