using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaTask.DAL;
using ProniaTask.Models;
using ProniaTask.ViewModel;

namespace ProniaTask.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {


        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _accessor;

        public HeaderViewComponent(AppDbContext context,IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            List<BasketItemVM> basketVM = new();
            if (Request.Cookies["Basket"] is not null)
            {
                List<BasketCookieItemVM> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                foreach (BasketCookieItemVM basketCookieItem in basket)
                {
                    Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == basketCookieItem.Id);
                    if (product is not null)
                    {
                        BasketItemVM basketItem = new()
                        {
                            Id = product.Id,
                            Name = product.Name,
                            ImageUrl = product.ProductImages.FirstOrDefault().ImageURL,
                            Price = product.Price,
                            Count = basketCookieItem.Count,
                            SubTotal = product.Price * basketCookieItem.Count,
                        };
                        basketVM.Add(basketItem);
                    }

                }

            }
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            HeaderVM vm = new() { Settings = settings, Basket = basketVM };

            return View(vm);
        }

    }
}
