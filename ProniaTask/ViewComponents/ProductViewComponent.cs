using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaTask.DAL;
using ProniaTask.Models;

namespace ProniaTask.ViewComponents
{
    public class ProductViewComponent:ViewComponent
    {
        private readonly AppDbContext _context;

        public ProductViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int key=1)
        {
            List<Product> products;
            switch (key)
            {
                case 1:
                    products = await _context.Products.Take(8).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).OrderBy(p=>p.Name).ToListAsync();
                    break;

                case 2:
                    products = await _context.Products.Take(8).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).OrderByDescending(p => p.SellCount).ToListAsync();
                    break;

                case 3:
                    products = await _context.Products.Take(8).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).OrderByDescending(p => p.Id).ToListAsync();
                    break;

                default:
                    products = await _context.Products.Take(8).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();
                    break;
            }


            
            return View(products);
        }
    }
}
