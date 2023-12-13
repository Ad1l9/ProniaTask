using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaTask.DAL;
using ProniaTask.DetailViewModel;
using ProniaTask.Models;
using ProniaTask.Utilities.Exceptions;

namespace ProniaTask.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            this._context = context;
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) throw new WrongRequestException("Gonderilen sorgu yanlisdir");

            Product product = await _context.Products
                .Include(p => p.Category)
                .Include(p=>p.ProductImages)
                .Include(p=>p.ProductTags).ThenInclude(p=>p.Tag)
                .Include(p=>p.ProductColors).ThenInclude(p=>p.Color)
                .Include(p=>p.ProductSizes).ThenInclude(p=>p.Size)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) throw new NotFoundException("Bele bir mehsul tapilmadi");

            List<Product> products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages.Where(pi=>pi.IsPrimary!=null))
                .Where(p => p.CategoryId == product.CategoryId && p.Id!=product.Id)
                .ToListAsync();


            DetailVM detailVM = new() { Product = product, RelatedProducts = products };
            return View(detailVM);
        }

        public IActionResult ErrorPage(string error = "Xeta bash verdi")
        {
            return View(model: error);
        }
    }
}
