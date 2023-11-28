using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaTask.Areas.ProniaAdmin.ViewModels;
using ProniaTask.DAL;
using ProniaTask.Models;

namespace ProniaTask.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products.Include(p=>p.Category).Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true)).ToListAsync();
            return View(products);
        }


        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View();
            }

            bool result = await _context.Categories.AnyAsync(c=>c.Id==productVM.CategoryId);

            if (!result)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ModelState.AddModelError("CategoryId","Bu id-li category movcud deyil");
                return View();
            }

            Product product = new()
            {
                Name = productVM.Name,
                Price = productVM.Price,
                SKU = productVM.SKU,
                CategoryId = (int)productVM.CategoryId,
                Description = productVM.Description
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
                .Include(pi => pi.ProductImages)
                .Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();
            return View(product);
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            var deleted = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);
            if (deleted is null) return NotFound();
            _context.Products.Remove(deleted);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();

            UpdateProductVM productVM = new()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                Categories = await _context.Categories.ToListAsync()
            };
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                return View();
            }
            Product existed = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();


            bool result = await _context.Products.AnyAsync(c=>c.Id==productVM.CategoryId);
            if (!result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                return View();
            }


            existed.Name = productVM.Name;
            existed.Description = productVM.Description;
            existed.Price = productVM.Price;
            existed.SKU = productVM.SKU;
            existed.CategoryId = productVM.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }
    }
}
